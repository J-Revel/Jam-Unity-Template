using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class SliderDisplayState
{
    public Sprite handle_sprite;
    public Sprite fill_sprite;
    public Sprite background_sprite;
    public Color handle_color = new Color(0, 0, 0, 1);
    public Color fill_color = new Color(0, 0, 0, 1);
    public Color background_color = new Color(0, 0, 0, 1);

    public static SliderDisplayState Merge(SliderDisplayState override_state, SliderDisplayState default_state)
    {
        SliderDisplayState result = new SliderDisplayState();
        result.handle_sprite = override_state.handle_sprite != null ? override_state.handle_sprite : default_state.handle_sprite;
        result.fill_sprite = override_state.fill_sprite != null ? override_state.fill_sprite : default_state.fill_sprite;
        result.background_sprite = override_state.background_sprite != null ? override_state.background_sprite : default_state.background_sprite;
        result.handle_color = (override_state.handle_color != Color.black) ? override_state.handle_color : default_state.handle_color;
        result.fill_color = (override_state.fill_color != Color.black) ? override_state.fill_color : default_state.fill_color;
        result.background_color = (override_state.background_color != Color.black) ? override_state.background_color : default_state.background_color;
        return result;
    }
}
public class FocusSlider : Slider 
{
    [System.Serializable]
    public class SliderDisplayStates
    {
        public SliderDisplayState default_state;
        public SliderDisplayState focus_state;
        public SliderDisplayState handle_pressed_state;

        public Sprite GetHandleSprite(bool focus, bool pressed)
        {
            Sprite sprite = default_state.handle_sprite;
            if (focus && focus_state.handle_sprite != null)
            {
                sprite = focus_state.handle_sprite;
            }
            if (pressed && handle_pressed_state.handle_sprite != null)
            {
                sprite = handle_pressed_state.handle_sprite;
            }
            return sprite;
        }
    }
    
    private FocusWidget focus_widget;
    private FocusMenu menu;
    public SliderDisplayStates display_states;

    public UnityEvent<float> on_value_changed;
    public UnityEvent on_focus_gain;
    public UnityEvent on_focus_lost;

    public RectTransform background_rect;
    private Image fill_image;
    private Image handle_image;
    private Image background_image;

    public float increments = 0.1f;
    
    private void Awake()
    {
        menu = GetComponentInParent<FocusMenu>(true);
    }

    private void UpdateDisplay(SliderDisplayState display_state)
    {
        fill_image.sprite = display_state.fill_sprite;
        fill_image.color = display_state.fill_color;
        handle_image.sprite = display_state.handle_sprite;
        handle_image.color = display_state.handle_color;
        background_image.sprite = display_state.background_sprite;
        background_image.color = display_state.background_color;
    }
    private void UpdateDisplay(bool focus, bool pressed)
    {
        SliderDisplayState state = display_states.default_state;
        if (focus)
            state = SliderDisplayState.Merge(display_states.focus_state, display_states.default_state);
        UpdateDisplay(state);
    }
    
    void Start()
    {
        if (fillRect != null)
            fill_image = fillRect.GetComponent<Image>();
        if (handleRect != null)
            handle_image = handleRect.GetComponent<Image>();
        if (background_rect != null)
            background_image = background_rect.GetComponent<Image>();
        focus_widget = GetComponentInParent<FocusWidget>();
        focus_widget.focus_gained_delegate += (stack) =>
        {
            UpdateDisplay(true, false);
            on_focus_gain.Invoke();
        };
        focus_widget.focus_lost_delegate += (stack) =>
        {
            UpdateDisplay(false, false);
            on_focus_lost.Invoke();
        };
        focus_widget.navigation_action_delegate += (action, originDirection) =>
        {
            switch (action)
            {
                case WidgetNavigationAction.None:
                    break;
                case WidgetNavigationAction.Up:
                    break;
                case WidgetNavigationAction.Down:
                    break;
                case WidgetNavigationAction.Left:
                    value -= increments;
                    return true;
                case WidgetNavigationAction.Right:
                    value += increments;
                    return true;
                case WidgetNavigationAction.GainFocus:
                    focus_widget.ForceFocus();
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }

            return false;
        };
    }
}
#if UNITY_EDITOR
    [CustomEditor(typeof(FocusSlider))]
    [CanEditMultipleObjects]
    public class FocusSliderEditor : Editor
    {
        private SerializedProperty fill_rect_prop, handle_rect_prop, display_states_prop, value_changed_prop, transition_prop, background_rect_prop;
        
        void OnEnable()
        {
            fill_rect_prop = serializedObject.FindProperty("m_FillRect");
            handle_rect_prop = serializedObject.FindProperty("m_HandleRect");
            background_rect_prop = serializedObject.FindProperty("background_rect");
            display_states_prop = serializedObject.FindProperty("display_states");
            value_changed_prop = serializedObject.FindProperty("m_OnValueChanged");
            transition_prop = serializedObject.FindProperty("m_Transition");
        }

        void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(fill_rect_prop);
            EditorGUILayout.PropertyField(handle_rect_prop);
            EditorGUILayout.PropertyField(background_rect_prop);
            EditorGUILayout.PropertyField(display_states_prop);
            EditorGUILayout.PropertyField(value_changed_prop);
            transition_prop.intValue = 0;

            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
