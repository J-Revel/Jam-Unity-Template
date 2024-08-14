using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum WidgetNavigationAction
{
    None, Up, Down, Left, Right, GainFocus,
}

public enum WidgetNavigationDirection
{
    None, Up, Down, Left, Right, 
}

public enum NavigationEventType
{
    Replace, Push, Pop, ReturnTo,
}

public delegate bool NavigationDelegate(WidgetNavigationAction action, WidgetNavigationDirection origin_direction);

public delegate bool ActionHandlingDelegate();

public struct NavigationEvent
{
    public FocusWidget target_widget;
    public WidgetNavigationAction action;
    public bool push;
}

public class FocusWidget : MonoBehaviour, IPointerEnterHandler
{
    private FocusMenu menu;

    public bool focus_on_hover = true;
    
    public System.Action<FocusWidget[]> focus_gained_delegate = (stack) => {};
    public System.Action<FocusWidget[]> focus_lost_delegate = (stack) => {};
    public System.Action<FocusWidget[]> focus_update_delegate = (stack) => {};
    public UnityEvent focus_gained_event;
    public UnityEvent focus_lost_event;
    public ActionHandlingDelegate confirm_delegate = () => { return false; };
    public NavigationDelegate navigation_action_delegate;

    public void Awake()
    {
        navigation_action_delegate = DefaultHandleNavigation;
        menu = GetComponentInParent<FocusMenu>(true);
    }

    public void Start()
    {
        menu = GetComponentInParent<FocusMenu>(true);
    }

    public bool DefaultHandleNavigation(WidgetNavigationAction action, WidgetNavigationDirection origin_direction)
    {
        if (action == WidgetNavigationAction.GainFocus)
        {
            ForceFocus();
            return true;
        }

        return false;
    }

    public void ForceFocus()
    {
        List<FocusWidget> stack = new List<FocusWidget>();
        for (Transform cursor = transform; cursor != null && cursor != menu.transform; cursor = cursor.parent)
        {
            FocusWidget focus_widget = cursor.GetComponent<FocusWidget>();
            if(focus_widget != null)
                stack.Add(focus_widget);
        }

        stack.Reverse();
        menu.ForceFocus(stack.ToArray());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(focus_on_hover)
            ForceFocus();
    }

    public bool has_focus => menu.focus_stack.Contains(this);
}
