using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepCollection : MonoBehaviour {

    private ScenarioStep[] steps;
    private int stepIndex;
    public bool orderMatters;
    public bool IsDone { get { return steps.Length == stepIndex; } }

    private void Awake()
    {
        steps = GetComponentsInChildren<ScenarioStep>();
    }

    public void InitiateNextStep()
    {
        steps[stepIndex].SetEnabled(false);
        Scenario.InitiateNextStep(ref stepIndex, steps);

        if (!IsDone)
        {
            steps[stepIndex].SetEnabled(true);
        }
    }

    public bool IsThisCurrentStep(ScenarioStep step)
    {
        return orderMatters ? steps[stepIndex] == step : true;
    }

    public bool HasStep(ScenarioStep step)
    {
        return Array.IndexOf(steps, step) != -1;
    }

    public void Initiate()
    {
        if(steps == null)
            steps = GetComponentsInChildren<ScenarioStep>(); 

        if (orderMatters && steps.Length > 0)
        {
            steps[0].SetEnabled(true);

            for(int i = 1; i < steps.Length; i++)
            {
                steps[i].SetEnabled(false);
            }
        }
        else
        {
            foreach (ScenarioStep step in steps)
            {
                step.gameObject.SetActive(true);
            }
        }
        
    }
}
