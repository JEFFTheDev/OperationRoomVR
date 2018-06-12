using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableCart : MonoBehaviour {

    private bool isMoving;
    private Transform hand;
    public float speed;
    public float wheelsRotateSpeed;
    public GameObject[] wheels;
    private Vector3 direction;
    private Vector3 lastPosition;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (hand)
        {
            transform.position = Vector3.MoveTowards(transform.position, hand.position, speed * Time.deltaTime);
        }

        direction = transform.position - lastPosition;

        lastPosition = transform.position;


        //Rotate wheels
        foreach (GameObject wheel in wheels)
        {
            Vector3 rot = Vector3.RotateTowards(transform.forward, direction, wheelsRotateSpeed * Time.deltaTime, 0f);
            wheel.transform.rotation = Quaternion.LookRotation(rot);
        }
    }

    void OnGrabEvent(Transform hand)
    {
        this.hand = hand;
    }

    void OnReleaseEvent()
    {
        hand = null;
    }
}
