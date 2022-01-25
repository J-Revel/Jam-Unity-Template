using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGameObjectScenarioAction : MonoBehaviour
{
    private ScenarioNode node;
    public GameObject target;
    public bool setActive;

    void Start()
    {
        node = GetComponent<ScenarioNode>();
        if(node == null)
            node = gameObject.AddComponent<ScenarioNode>();
        node.enterNodeDelegate += () => {
            target.SetActive(setActive);
        };
    }
}
