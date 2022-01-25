using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** the base component of scenario, controlled by the ScenarioManager. Provides delegates that scenario components can use :
        - enterNodeDelegate triggered when the scenario enters the node
        - conditionDelegate : components bound to this delegate must return a boolean. 
        At each frame, the ScenarioManager checks the result of all functions bound to this delegate, and allow to leave the node only when all return true
        - leaveNodeDelegate triggered when the scenario leaves the node
 **/
public class ScenarioNode : MonoBehaviour
{
    public bool activeNode = false;
    public delegate void ScenarioNodeDelegate();
    public delegate bool ConditionDelegate();

    public ScenarioNodeDelegate enterNodeDelegate;
    public ScenarioNodeDelegate leaveNodeDelegate;
    public ConditionDelegate conditionDelegate;
    public System.Action forceExitDelegate;

    public bool IsRequirementMet()
    {
        bool result = true;
        if (conditionDelegate != null)
        {
            foreach (ConditionDelegate boundElement in conditionDelegate.GetInvocationList())
                result &= boundElement();
        }
        return result;
    }

    public void EnterNode()
    {
        activeNode = true;
        if (enterNodeDelegate != null)
            enterNodeDelegate();
    }

    public void LeaveNode()
    {
        activeNode = false;
        if (leaveNodeDelegate != null)
            leaveNodeDelegate();
    }
}
