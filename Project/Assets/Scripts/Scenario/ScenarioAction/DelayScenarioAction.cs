using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayScenarioAction : MonoBehaviour
{
    private ScenarioNode node;
    public float duration = 1;
    private float time = 0;
    private bool insideNode = false;

    void Start()
    {
        node = GetComponent<ScenarioNode>();
        if(node == null)
            node = gameObject.AddComponent<ScenarioNode>();
        node.enterNodeDelegate += () => {
            insideNode = true;
            time = 0;
        };
        node.leaveNodeDelegate += () => {
            insideNode = false;
            time = 0;
        };
        node.conditionDelegate += CanExitNode;
    }

    private void Update()
    {
        if(insideNode)
        {
            time += Time.deltaTime;
        }
    }

    private bool CanExitNode()
    {
        return insideNode && time >= duration;
    }
}
