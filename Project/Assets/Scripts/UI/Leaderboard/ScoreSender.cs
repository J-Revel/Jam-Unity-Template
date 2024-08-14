using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreSender : MonoBehaviour
{
    public UnityEvent score_sent_event;
    public void SendScore()
    {
        ScoreSystem.instance.SendScore(() =>
        {
            score_sent_event.Invoke();
        });
    }
}
