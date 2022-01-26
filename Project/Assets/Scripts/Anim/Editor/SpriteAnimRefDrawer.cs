using UnityEditor;
using UnityEngine;
using System;

[CustomPropertyDrawer( typeof( SpriteAnimRef ) )]
public class SpriteAnimRefDrawer : PropertyDrawer {

    public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) {
        // The 6 comes from extra spacing between the fields (2px each)
        return EditorGUIUtility.singleLineHeight * 2 + 6;
    }

    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
        // EditorGUI.BeginProperty( position, label, property );

        // EditorGUI.LabelField( position, label );

        // EditorGUI.indentLevel++;

        // var nameRect = EditorGUILayout.BeginHorizontal ();
        // EditorGUI.PropertyField( nameRect, property.FindPropertyRelative( "animName" ), GUIContent.none);
        // EditorGUILayout.EndHorizontal();
        // var nameRect = EditorGUILayout.BeginHorizontal ();
        Rect libRect = position;
        libRect.height = EditorGUIUtility.singleLineHeight;
        Rect typeRect = libRect;
        typeRect.y += EditorGUIUtility.singleLineHeight;
        SerializedProperty animListProp = property.FindPropertyRelative("animList");
        SerializedProperty animNameProp = property.FindPropertyRelative("animName");
        EditorGUI.PropertyField(libRect, animListProp);
        SpriteAnimList animList = (SpriteAnimList)animListProp.objectReferenceValue;
        string selectedName = animNameProp.stringValue;
        int selectedIndex = -1;
        if(animList != null)
        {
            string[] options = new string[animList.spriteAnims.Length];
            for(int i=0; i<animList.spriteAnims.Length; i++)
            {
                options[i] = animList.spriteAnims[i].name;
                if(options[i] == selectedName)
                {
                    selectedIndex = i;
                }
            }
            int newSelectedIndex = EditorGUI.Popup(typeRect, Mathf.Max(0, selectedIndex), options);
            if(newSelectedIndex != selectedIndex)
            {
                animNameProp.stringValue = options[newSelectedIndex];
            }
        }
        // EditorGUILayout.EndHorizontal();

        // EditorGUI.indentLevel--;

        // EditorGUI.EndProperty();
    }
}