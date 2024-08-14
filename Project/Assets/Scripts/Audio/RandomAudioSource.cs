using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RandomAudioSource: MonoBehaviour
{
    public SoundConfig config;
    void Start()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.volume = Random.Range(config.volume_min, config.volume_max);
        source.pitch = Random.Range(config.pitch_default - config.pitch_random / 2, config.pitch_default + config.pitch_random / 2);
        source.clip = config.clips[Random.Range(0, config.clips.Length)];
        source.outputAudioMixerGroup = config.mixer_group;
        source.Play();
    }
}
