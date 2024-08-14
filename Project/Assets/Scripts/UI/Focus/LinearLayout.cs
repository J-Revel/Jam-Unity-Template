using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LinearLayoutDirection
{
    Horizontal,
    Vertical,
}

[RequireComponent(typeof(FocusWidget))]
public class LinearLayout : MonoBehaviour
{
    public LinearLayoutDirection direction;
    private FocusWidget widget;
    public Transform layout_container;
    private int cursor = 0;
    private FocusWidget[] children;
    private FocusMenu menu;

    void Awake()
    {
        menu = GetComponentInParent<FocusMenu>(true);
    }
    void Start()
    {
        List<FocusWidget> children_list = new List<FocusWidget>();

        Transform container = layout_container;
        if (container == null)
            container = transform;
        for (int i = 0; i < container.childCount; i++) {
            FocusWidget child_widget = container.GetChild(i).GetComponent<FocusWidget>();
            if (child_widget != null)
            {
                children_list.Add(child_widget);
            }
        }

        children = children_list.ToArray();
        widget = GetComponent<FocusWidget>();

        widget.focus_gained_delegate = (stack) =>
        {
            for (int i = 0; i < menu.focus_stack.Count; i++)
            {
                if (stack[i] == widget)
                {
                    FocusWidget selected_child = stack[i + 1];
                    for (int j = 0; j < children.Length; j++)
                    {
                        if (children[j] == selected_child)
                        {
                            cursor = j;
                        }
                    }
                }
            }
        };
        
        widget.focus_update_delegate = (stack) =>
        {
            for (int i = 0; i < stack.Length; i++)
            {
                if (stack[i].gameObject == this.gameObject)
                {
                    FocusWidget selected_child = stack[i + 1];
                    for (int j = 0; j < children.Length; j++)
                    {
                        if (children[j] == selected_child)
                        {
                            cursor = j;
                        }
                    }
                }
            }
        };
        
        widget.navigation_action_delegate = (WidgetNavigationAction action, WidgetNavigationDirection origin_direction) =>
        {
            switch (action)
            {
                case WidgetNavigationAction.None:
                    return true;
                    break;
                case WidgetNavigationAction.Up:
                    if (direction == LinearLayoutDirection.Vertical)
                        return HandleLayoutNavigation(-1, origin_direction);
                    break;
                case WidgetNavigationAction.Down:
                    if(direction == LinearLayoutDirection.Vertical)
                        return HandleLayoutNavigation(1, origin_direction);
                    break;
                case WidgetNavigationAction.Left:
                    if (direction == LinearLayoutDirection.Horizontal)
                        return HandleLayoutNavigation(-1, origin_direction);
                    break;
                case WidgetNavigationAction.Right:
                    if (direction == LinearLayoutDirection.Horizontal)
                        return HandleLayoutNavigation(1, origin_direction);
                    break;
                case WidgetNavigationAction.GainFocus:
                    switch (direction)
                    {
                        case LinearLayoutDirection.Horizontal:
                            if (origin_direction == WidgetNavigationDirection.Left)
                            {
                                cursor = children.Length - 1;
                            }
                            else if (origin_direction == WidgetNavigationDirection.Right)
                            {
                                cursor = 0;
                            }
                            break;
                        case LinearLayoutDirection.Vertical:
                            if (origin_direction == WidgetNavigationDirection.Up)
                            {
                                cursor = children.Length - 1;
                            }
                            else if (origin_direction == WidgetNavigationDirection.Down)
                            {
                                cursor = 0;
                            }
                            break;
                    }
                    return children[cursor].navigation_action_delegate(WidgetNavigationAction.GainFocus, origin_direction);
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }

            return false;
        };
    }

    private bool HandleLayoutNavigation(int direction, WidgetNavigationDirection navigation_direction)
    {
        cursor += direction;
        if (cursor < 0)
        {
            cursor = 0;
        }

        if (cursor >= children.Length)
        {
            cursor = children.Length - 1;
        }
        
        children[cursor].navigation_action_delegate(WidgetNavigationAction.GainFocus, navigation_direction);
        return true;
    }
}
