using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
    public SoundConfig soundConfig;
    
}

[System.Serializable]
public struct SoundConfig
{
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;

    public float minPitch;
    public float maxPitch;
    public float minIntensity;
    public float maxIntensity;
}

public class EventTable : MonoBehaviour
{
    public AudioSource audioPrefab;
    public EventConfig[] events;

    private void Start()
    {
        foreach(EventConfig eventConfig in events)
        {
            EventInvocationData invocationData = eventConfig.invocationData;
            Component component = invocationData.gameObject.GetComponent(invocationData.scriptType);
            
            System.Reflection.FieldInfo field = component.GetType().GetField(invocationData.fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            object target = field.GetValue(component);
            if(field.FieldType.IsSubclassOf(typeof(UnityEngine.Events.UnityEvent)))
            {
                (target as UnityEngine.Events.UnityEvent).AddListener(() => {
                    StartCoroutine(PlaySound(component.gameObject, eventConfig.soundConfig));
                });
            }
            if(field.FieldType.IsSubclassOf(typeof(System.MulticastDelegate)))
            {
                var targetDelegate = (target as System.Action);
                targetDelegate += () => {
                    StartCoroutine(PlaySound(component.gameObject, eventConfig.soundConfig));
                };
            }
        }
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
                gameObjectPath = parent.name + "/" + gameObjectPath;
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

    public IEnumerator PlaySound(GameObject emitter, SoundConfig soundConfig)
    {
        AudioSource source = Instantiate(audioPrefab, emitter.transform.position, emitter.transform.rotation);
        source.volume = Random.Range(soundConfig.minIntensity, soundConfig.maxIntensity);
        source.pitch = Random.Range(soundConfig.minPitch, soundConfig.maxPitch);
        source.clip = soundConfig.clip;
        source.outputAudioMixerGroup = soundConfig.mixerGroup;
        source.Play();
        yield return new WaitForSeconds(soundConfig.clip.length + 1);
        Destroy(source.gameObject);
    }
}
