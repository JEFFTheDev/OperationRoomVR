using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Scenario : MonoBehaviour {

    private static Scenario instance;
    public static Scenario Instance { get { return instance; } private set { instance = value; } }
    public StepCollection[] steps;
    private int collectionIndex;
    private bool IsDone { get { return steps.Length == collectionIndex; } }
    
    // Use this for initialization
    void Start () {
        Instance = this;

        InitiateCollection(steps[0]);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitiateCollection(StepCollection collection)
    {
        collection.gameObject.SetActive(true);
        collection.Initiate();
    }

    public bool IsCurrentStep(ScenarioStep step)
    {
        return steps[collectionIndex].IsThisCurrentStep(step);
    }

    public void InitiateNextStep()
    {
        steps[collectionIndex].InitiateNextStep();

        if (steps[collectionIndex].IsDone)
        {
            InitiateNextStep(ref collectionIndex, steps);

            if (!IsDone)
            {
                InitiateCollection(steps[collectionIndex]);
            }
        }
    }

    public static void InitiateNextStep(ref int index, object[] steps)
    {
        if (steps.Length != index)
            index++;
    }

    public void Snappppppppp()
    {
        Debug.Log("snaaaaaaaaaap");
    }

    public void Timeeeeeeeee()
    {
        Debug.Log("Timeeeeeeeee");
    }
}