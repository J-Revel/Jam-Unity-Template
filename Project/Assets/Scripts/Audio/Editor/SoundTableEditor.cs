using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundTable))]
public class SoundTableEditor : Editor
{
    SerializedProperty lookAtPoint;
    private bool hideUnused = false;
    
    void OnEnable()
    {
        lookAtPoint = serializedObject.FindProperty("lookAtPoint");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty eventsProperty = serializedObject.FindProperty("events");
        SoundTable eventTable = target as SoundTable;
        hideUnused = GUILayout.Toggle(hideUnused, "Hide unused");
        serializedObject.ApplyModifiedProperties();
        foreach(EventInvocationData invocationData in SoundTable.ListEvents(eventTable.gameObject))
        {
            if(hideUnused)
            {
                bool show = false;
                for(int i=0; i<eventsProperty.arraySize; i++)
                {
                    SerializedProperty invocationDataProperty = eventsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("invocationData");
                    if(invocationDataProperty.FindPropertyRelative("gameObject").objectReferenceValue == invocationData.gameObject
                        && invocationDataProperty.FindPropertyRelative("fieldName").stringValue == invocationData.fieldName)
                    {
                        show = true;
                        break;
                    }
                }
                if(!show)
                    continue;
            }
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(EditorStyles.boldLabel);
            var style = EditorStyles.boldLabel;
            style.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField(invocationData.displayPath, style, GUILayout.ExpandWidth(true));
            if(GUILayout.Button("+", GUILayout.Width(30)))
            {
                int insertIndex = Mathf.Max(eventsProperty.arraySize - 1, 0);
                eventsProperty.InsertArrayElementAtIndex(insertIndex);
                SerializedProperty newEventProperty = eventsProperty.GetArrayElementAtIndex(insertIndex);
                SerializedProperty invocationDataProperty = newEventProperty.FindPropertyRelative("invocationData");
                invocationDataProperty.FindPropertyRelative("displayPath").stringValue = invocationData.displayPath;
                invocationDataProperty.FindPropertyRelative("gameObject").objectReferenceValue = invocationData.gameObject;
                invocationDataProperty.FindPropertyRelative("scriptType").stringValue = invocationData.scriptType;
                invocationDataProperty.FindPropertyRelative("fieldName").stringValue = invocationData.fieldName;
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            for(int i=0; i<eventsProperty.arraySize; i++)
            {
                SerializedProperty invocationDataProperty = eventsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("invocationData");
                if(invocationDataProperty.FindPropertyRelative("gameObject").objectReferenceValue == invocationData.gameObject
                    && invocationDataProperty.FindPropertyRelative("fieldName").stringValue == invocationData.fieldName)
                {
                    var soundConfigProperty = eventsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("soundConfig");
                    EditorGUILayout.PropertyField(soundConfigProperty);
                    var buttonStyle = EditorStyles.boldLabel;
                    buttonStyle.alignment = TextAnchor.MiddleRight;
                    EditorGUILayout.BeginHorizontal(buttonStyle);
                    if(GUILayout.Button("Delete"))
                    {
                        eventsProperty.DeleteArrayElementAtIndex(i);
                    }
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.EndHorizontal();
                }

            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
    }
}
