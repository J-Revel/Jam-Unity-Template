using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** A scenario that is not launched at start but when a public function is called. Used for some minigames start / end **/
public class ScenarioManager : MonoBehaviour
{
    private Stack<int> stack = new Stack<int>();
    private Transform cursor;

    
    public System.Action scenarioFinishedDelegate;
    public UnityEngine.Events.UnityEvent scenarioFinishedEvent;

    public bool isScenarioFinished
    {
        get
        {
            return scenarioFinished;
        }
    }

    public void ResetScenario()
    {
        scenarioFinished = false;
    }

    public bool scenarioFinished;

    private void Awake()
    {
        stack.Clear();
        cursor = transform;
    }

    public void StartScenario()
    {
        scenarioFinished = false;
        EnterFirstNode();
    }

    private void EnterFirstNode()
    {
        if (cursor.childCount == 0)
        {
            if (scenarioFinishedDelegate != null)
                scenarioFinishedDelegate();
            scenarioFinishedEvent?.Invoke();
            scenarioFinished = true;
        }
        cursor = cursor.GetChild(0);
        if (cursor == null)
            return;

        stack.Push(0);
        ScenarioNode node = cursor.GetComponent<ScenarioNode>();
        if (node == null)
            EnterNode(0);
        else node.EnterNode();
    }

    private void EnterNode(int index)
    {
        cursor = cursor.GetChild(index);
        if (cursor == null)
            return;
        stack.Push(index);
        ScenarioNode node = cursor.GetComponent<ScenarioNode>();
        if (node == null)
            EnterNode(0);
        else node.EnterNode();
    }

    private void LeaveNode()
    {
        while (cursor != transform && stack.Count > 0)
        {
            int lastIndex = stack.Pop();
            ScenarioNode node = cursor.GetComponent<ScenarioNode>();
            if (node != null)
                node.LeaveNode();
            cursor = cursor.parent;
            if (cursor.childCount > lastIndex + 1)
            {
                EnterNode(lastIndex + 1);
                return;
            }
        }
        if (cursor == transform)
        {
            if (scenarioFinishedDelegate != null)
                scenarioFinishedDelegate();
            scenarioFinished = true;
        }
    }

    private void Update()
    {
        if (cursor != transform)
        {
            if (cursor.GetComponent<ScenarioNode>().IsRequirementMet())
            {
                LeaveNode();
            }
        }
    }
}
