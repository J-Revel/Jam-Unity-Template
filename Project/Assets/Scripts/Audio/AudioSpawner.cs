using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSpawner : MonoBehaviour
{
    public AudioSource prefab;
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;

    public bool spawnAtStart = false;
    public float minPitch = 0;
    public float maxPitch = 0;
    public float minIntensity = 1;
    public float maxIntensity = 1;

    public void PlaySound()
    {
        AudioSource source = Instantiate(prefab, transform.position, transform.rotation);
        source.volume = Random.Range(minIntensity, maxIntensity);
        source.pitch = Random.Range(minPitch, maxPitch);
        source.clip = clip;
        source.outputAudioMixerGroup = mixerGroup;
        source.Play();
    }
}
