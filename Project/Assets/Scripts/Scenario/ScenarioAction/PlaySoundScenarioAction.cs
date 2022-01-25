using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundScenarioAction : MonoBehaviour
{
    private ScenarioNode node;
    public AudioSource source;
    public AudioClip clip;
    public bool waitForSoundEnd;
    private float time;
    private bool playing = false;

    void Start()
    {
        node = GetComponent<ScenarioNode>();
        if(node == null)
            node = gameObject.AddComponent<ScenarioNode>();
        node.enterNodeDelegate += () => {
            source.clip = clip;
            source.Play();
            time = 0;
            playing = waitForSoundEnd;
        };
        node.leaveNodeDelegate += () => {
            source.Stop();
        };
        node.conditionDelegate += CanExitNode;
    }

    void Update()
    {
        if(playing)
        {
            time += Time.deltaTime;
        }
    }

    bool CanExitNode()
    {
        return !waitForSoundEnd || (playing && time > clip.length);
    }
}
