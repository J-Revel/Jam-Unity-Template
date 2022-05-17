using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventTable))]
public class EventTableEditor : Editor
{
    SerializedProperty lookAtPoint;
    
    void OnEnable()
    {
        lookAtPoint = serializedObject.FindProperty("lookAtPoint");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EventTable eventTable = target as EventTable;
        foreach(EventInvocationData invocationData in EventTable.ListEvents(eventTable.gameObject))
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
            EditorGUILayout.LabelField(invocationData.displayPath, style, GUILayout.ExpandWidth(true));
            SerializedProperty eventsProperty = serializedObject.FindProperty("events");
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
            for(int i=0; i<eventsProperty.arraySize; i++)
            {
                if(eventsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("invocationData").FindPropertyRelative("displayPath").stringValue == invocationData.displayPath)
                {
                    var messageProperty = eventsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("message");
                    string newValue = EditorGUILayout.TextField(messageProperty.stringValue);
                    if(newValue != messageProperty.stringValue)
                    {
                        messageProperty.stringValue = newValue;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
