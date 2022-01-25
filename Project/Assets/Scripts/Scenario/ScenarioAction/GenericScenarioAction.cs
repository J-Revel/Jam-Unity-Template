using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericScenarioAction : MonoBehaviour
{
    private ScenarioNode node;
    public UnityEvent enterNodeEvent;
    public UnityEvent leaveNodeEvent;

    void Start()
    {
        node = GetComponent<ScenarioNode>();
        if(node == null)
            node = gameObject.AddComponent<ScenarioNode>();
        node.enterNodeDelegate += () => { enterNodeEvent?.Invoke(); };
        node.leaveNodeDelegate += () => { leaveNodeEvent?.Invoke(); };
    }
}
