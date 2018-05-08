using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintAnimation : MonoBehaviour {

    public Transform other;
    public float minDistance;
    public float animationSpeed;
    private Vector3 startPos;

	// Use this for initialization
	void Start () {
        startPos = other.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (IsWithinDistance())
            other.transform.position = startPos;

        other.transform.position = Vector3.Lerp(other.transform.position, transform.position, animationSpeed * Time.deltaTime);	
	}

    private bool IsWithinDistance()
    {
        return Vector3.Distance(transform.position, other.position) < minDistance;
    }
}
