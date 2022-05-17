using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EventInvocationData
{
    public string displayPath;
    public GameObject gameObject;
    public string scriptType;
    public string fieldName;
}

[System.Serializable]
public struct EventConfig
{
    public EventInvocationData invocationData;
    public string message;
}

public class EventTable : MonoBehaviour
{
    public EventConfig[] events;

    private void Start()
    {
        foreach(EventConfig eventConfig in events)
        {
            EventInvocationData invocationData = eventConfig.invocationData;
            Component component = invocationData.gameObject.GetComponent(invocationData.scriptType);
            Debug.Log(invocationData.fieldName);
            System.Reflection.FieldInfo field = component.GetType().GetField(invocationData.fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object target = field.GetValue(component);
            if(field.FieldType.IsSubclassOf(typeof(UnityEngine.Events.UnityEvent)))
            {
                string message = eventConfig.message;
                (target as UnityEngine.Events.UnityEvent).AddListener(() => {
                    OnEventTriggered(message);
                });
            }
            if(field.FieldType.IsSubclassOf(typeof(System.MulticastDelegate)))
            {
                string message = eventConfig.message;
                var targetDelegate = (target as System.Action);
                targetDelegate += () => {
                    OnEventTriggered(message);
                };
            }
        }
    }

    private void OnEventTriggered(string message)
    {
        Debug.Log(message);
    }

    private void Update()
    {
    }

    public static EventInvocationData[] ListEvents(GameObject gameObject)
    {
        List<EventInvocationData> result = new List<EventInvocationData>();
        MonoBehaviour[] childrenBehaviour = gameObject.GetComponentsInChildren<MonoBehaviour>();
        foreach(MonoBehaviour behaviour in childrenBehaviour)
        {
            System.Type type = behaviour.GetType();
            string gameObjectPath = "";
            for(Transform parent = behaviour.transform; parent != gameObject.transform; parent = parent.parent)
            {
                gameObjectPath += parent.name + "/";
            }
            foreach(var member in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            {
                if(member.FieldType.IsSubclassOf(typeof(UnityEngine.Events.UnityEvent)) || member.FieldType.IsSubclassOf(typeof(System.MulticastDelegate)))
                {
                    EventInvocationData invocationData = new EventInvocationData();
                    invocationData.displayPath = gameObjectPath + type.Name + "." + member.Name;
                    invocationData.gameObject = behaviour.gameObject;
                    invocationData.fieldName = member.Name;
                    invocationData.scriptType = type.FullName;
                    result.Add(invocationData);
                }
            }
        }

        return result.ToArray();
    }
}
