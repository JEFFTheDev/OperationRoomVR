using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScenarioStep : MonoBehaviour
{
    private string acceptName;
    public bool isDone;
    public float disableAfterTimer;
    public UnityEvent onSnap;
    public UnityEvent onTimerEnd;
    private Transform currentSnapped;

    // Use this for initialization
    void Start()
    {
        acceptName = name;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSnapped)
            currentSnapped.SetPositionAndRotation(transform.position, transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.name.Substring(0, 5) == acceptName.Substring(0, 5))
        {
            if (Scenario.Instance.IsCurrentStep(this))
            {
                onSnap.Invoke();
                SetEnabled(false);
                currentSnapped = other.transform;
                StartCoroutine(Timer(disableAfterTimer));
            }
                
        }
    }

    public void SetEnabled(bool enable)
    {
        foreach (MeshRenderer mr in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = enable;
        }

        gameObject.GetComponent<Collider>().enabled = enable;
    }

    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);
        isDone = true;
        Scenario.Instance.InitiateNextStep();
        onTimerEnd.Invoke();

        if (time > 0)
            Destroy(currentSnapped.gameObject);
    }
}
