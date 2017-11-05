using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgent : Agent
{
    private List<float> currentState;
    public override void InitializeAgent()
    {
        currentState = new List<float>();
        AgentReset();
    }

    public override void AgentReset()
    {
        currentState.Clear();

        for (int i = 0; i < 10; i++)
            currentState.Add(i / 10.0f);

        int n = currentState.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            float value = currentState[k];
            currentState[k] = currentState[n];
            currentState[n] = value;
        }

    }

    public override List<float> CollectState()
    {
        List<float> state = new List<float>(currentState);
        return state;
    }

    public override void AgentStep(float[] act)
    {
        int action = (int)act[0];

        string actionDesc = "";

        if (action < 0 || action > currentState.Count)
        {
            throw new UnityAgentsException("invalid action: " + action);
        }

        if (currentState[action] > 0.69 && currentState[action] < 0.71)
        {
            actionDesc = "Agent found 7 under index " + action + " in List: " + string.Join(",", currentState.ToArray());
            reward = 1;
           // done = true;
        }
        else
        {
            actionDesc = " Agent did not found 7 with action " + action + " in List: " + string.Join(",", currentState.ToArray()) + ", debug: " + string.Join(",", act);

            reward = -1;
            done = true;
        }

        Debug.Log(actionDesc);
    }

    public override void AgentOnDone()
    {

    }
}