using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FocusMenu : MonoBehaviour
{
    private static FocusMenu active_menu;
    public bool auto_take_focus = true;
    public FocusWidget initial_focus;
    public List<FocusWidget> focus_stack = new List<FocusWidget>();
    private InputConfig input_config;

    public bool confirm_action_down = false;

    public bool navigation_enabled = true;
    
    void Start()
    {
        active_menu = this;
        initial_focus.ForceFocus();
        input_config = new InputConfig();
        input_config.Navigation.Right.performed += callback_context =>
        {
            HandleNavigationAction(WidgetNavigationAction.Right, WidgetNavigationDirection.Right);
        };
        input_config.Navigation.Left.performed += callback_context =>
        {
            HandleNavigationAction(WidgetNavigationAction.Left, WidgetNavigationDirection.Left);
        };
        input_config.Navigation.Up.performed += callback_context =>
        {
            HandleNavigationAction(WidgetNavigationAction.Up, WidgetNavigationDirection.Up);
        };
        input_config.Navigation.Down.performed += callback_context =>
        {
            HandleNavigationAction(WidgetNavigationAction.Down, WidgetNavigationDirection.Down);
        };
        input_config.Navigation.Confirm.performed += callback_context =>
        {
            HandleConfirmState();
        };
        input_config.Enable();
    }

    public void ForceFocus(FocusWidget new_focus)
    {
        List<FocusWidget> stack = new List<FocusWidget>();
        for (Transform cursor = new_focus.transform; cursor != null && cursor != transform; cursor = cursor.parent)
        {
            FocusWidget focus_widget = cursor.GetComponent<FocusWidget>();
            if(focus_widget != null)
                stack.Add(focus_widget);
        }

        stack.Reverse();
        ForceFocus(stack.ToArray());
    }

    public void ForceFocus(FocusWidget[] new_stack)
    {
        if (!navigation_enabled)
            return;
        foreach (FocusWidget old_stack_element in focus_stack)
        {
            if (!new_stack.Contains(old_stack_element))
            {
                old_stack_element.focus_lost_delegate(new_stack);
                old_stack_element.focus_lost_event.Invoke();
            }
            else old_stack_element.focus_update_delegate(new_stack);
        }
        foreach (FocusWidget new_stack_element in new_stack)
        {
            if (!focus_stack.Contains(new_stack_element))
            {
                new_stack_element.focus_gained_delegate(new_stack);
                new_stack_element.focus_gained_event.Invoke();
            }
        }

        focus_stack.Clear();
        focus_stack.AddRange(new_stack);
    }

    public void HandleNavigationAction(WidgetNavigationAction action, WidgetNavigationDirection origin_direction)
    {
        if (!navigation_enabled)
            return;
        for (int i = focus_stack.Count - 1; i >= 0; i--)
        {
            if (focus_stack[i].navigation_action_delegate(action, origin_direction))
                return;
        }
    }
    
    public void HandleConfirmState()
    {
        if (!navigation_enabled)
            return;
        for (int i = focus_stack.Count - 1; i >= 0; i--)
        {
            if (focus_stack[i].confirm_delegate())
                return;
        }
    }

    void Update()
    {
        confirm_action_down = input_config.Navigation.Confirm.IsPressed();
        if (active_menu == this)
        {
            if (focus_stack.Count > 0)
            {
                //focus_stack[^1].navigation_action_delegate();
            }
        }
    }
}