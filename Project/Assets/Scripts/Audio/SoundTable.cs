using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;


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
public class SoundConfig
{
    public AudioClip[] clips;
    [FormerlySerializedAs("mixerGroup")] public AudioMixerGroup mixer_group;

    public float pitch_default = 1;
    public float pitch_random = 0;
    [Range(0, 1)] public float volume_min = 1;
    [Range(0, 1)] public float volume_max = 0;
    public float pitch_param_effect = 0;
    public float volume_param_effect = 0;
}

[System.Serializable]
public class SoundEvent
{
    public System.Action<SoundEvent> event_delegate;
    public float param_value;

    public void Trigger()
    {
        event_delegate?.Invoke(this);
    }
}

public class SoundTable : MonoBehaviour
{
    private AudioSource audioPrefab;
    public EventConfig[] events;

    private void Start()
    {
        audioPrefab = Resources.Load<AudioSource>("Spawned Sound");
        foreach(EventConfig eventConfig in events)
        {
            EventInvocationData invocationData = eventConfig.invocationData;
            Component component = invocationData.gameObject.GetComponent(invocationData.scriptType);
            
            System.Reflection.FieldInfo field = component.GetType().GetField(invocationData.fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if(field != null)
            {
                object target = field.GetValue(component);
                if(field.FieldType == typeof(SoundEvent))
                {
                    (target as SoundEvent).event_delegate += (SoundEvent sound_event) => {
                        StartCoroutine(PlaySound(component.gameObject, eventConfig.soundConfig, sound_event));
                    };
                }
            }
        }
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
                if(member.FieldType == typeof(SoundEvent))
                {
                    EventInvocationData invocationData = new EventInvocationData
                    {
                        displayPath = gameObjectPath + type.Name + "." + member.Name,
                        gameObject = behaviour.gameObject,
                        fieldName = member.Name,
                        scriptType = type.FullName
                    };
                    result.Add(invocationData);
                }
            }
        }

        return result.ToArray();
    }

    public IEnumerator PlaySound(GameObject emitter, SoundConfig soundConfig, SoundEvent sound_event)
    {
        AudioSource source = Instantiate(audioPrefab, emitter.transform.position, emitter.transform.rotation);
        float volume = Random.Range(soundConfig.volume_min, soundConfig.volume_max);
        float pitch = Random.Range(soundConfig.pitch_default - soundConfig.pitch_random / 2, soundConfig.pitch_default + soundConfig.pitch_random / 2);
        source.volume = volume + sound_event.param_value * soundConfig.volume_param_effect;
        source.pitch = pitch + sound_event.param_value * soundConfig.pitch_param_effect;
        AudioClip selectedClip = soundConfig.clips[Random.Range(0, soundConfig.clips.Length)];
        source.clip = selectedClip;
        source.outputAudioMixerGroup = soundConfig.mixer_group;
        source.Play();
        for (float time = 0; time < selectedClip.length + 1; time += Time.deltaTime)
        {
            source.volume = volume + sound_event.param_value * soundConfig.volume_param_effect;
            source.pitch = pitch + sound_event.param_value * soundConfig.pitch_param_effect;
            yield return null;
        }
        Destroy(source.gameObject);
    }
}
