using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
#endif

[RequireComponent(typeof(Image)), ExecuteAlways]
public class FocusImage : MonoBehaviour
{
    private Image image;
    private FocusWidget focus_widget;

    public Sprite default_sprite;
    public Color default_color = Color.white;
    public Sprite focus_sprite;
    public Color focus_color = Color.white; 
    void Start()
    {
        image = GetComponent<Image>();
        image.sprite = default_sprite;
        image.color = default_color;
        focus_widget = GetComponentInParent<FocusWidget>();
        focus_widget.focus_gained_delegate += (stack) =>
        {
            image.sprite = focus_sprite;
            image.color = focus_color;
        };
        focus_widget.focus_lost_delegate += (stack) =>
        {
            image.sprite = default_sprite;
            image.color = default_color;
        };
    }
}

#if UNITY_EDITOR
    [CustomEditor(typeof(FocusImage))]
    [CanEditMultipleObjects]
    public class FocusImageEditor : Editor
    {
        private bool display_focus;
        private Image target_image;
        void OnEnable()
        {
            target_image = (target as FocusImage).GetComponent<Image>();
            FocusImage focus_image = (target as FocusImage);
            target_image.color = display_focus ? focus_image.focus_color : focus_image.default_color;
            target_image.sprite = display_focus ? focus_image.focus_sprite: focus_image.default_sprite;
        }

        void OnDisable()
        {
            FocusImage focus_image = (target as FocusImage);
            target_image.color = focus_image.default_color;
            target_image.sprite = focus_image.default_sprite;
            
        }
    
        public override void OnInspectorGUI()
        {
            FocusImage focus_image = (target as FocusImage);
            if (!display_focus && GUILayout.Button("Default State"))
            {
                display_focus = true;
                target_image.color = display_focus ? focus_image.focus_color : focus_image.default_color;
                target_image.sprite = display_focus ? focus_image.focus_sprite: focus_image.default_sprite;
                SceneView.RepaintAll();
            }
            else if (display_focus && GUILayout.Button("Focused State"))
            {
                display_focus = false;
                target_image.color = display_focus ? focus_image.focus_color : focus_image.default_color;
                target_image.sprite = display_focus ? focus_image.focus_sprite: focus_image.default_sprite;
                SceneView.RepaintAll();
            }
            base.OnInspectorGUI();
        }
    }
#endif
