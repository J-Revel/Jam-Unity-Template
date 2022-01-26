using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSound : MonoBehaviour
{
    private AudioSource source;
    private float time;
    void Start()
    {
        source = GetComponent<AudioSource>();
        time = source.clip.length;
    }

    void Update()
    {
        time -= Time.deltaTime;
        if(time <= 0)
            Destroy(gameObject);
    }
}
