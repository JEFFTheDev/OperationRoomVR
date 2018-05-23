using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactible : MonoBehaviour
{


    public UnityEvent onGrab;
    public UnityEvent onRelease;
    public UnityEvent onHold;
    public string[] tags;
    private GameObject collidedWith;
    private bool isGrabbed;
    public Vector3 startPos;
    private Shader startShader;
    private bool isHighlighted;
    private Collider currentCol;

    // Use this for initialization
    void Start()
    {
        startShader = GetComponent<MeshRenderer>().material.shader;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrabbed)
        {
            if (IsGrabbing())
            {
                Hold();
            }
            else
            {
                Release();
            }
        }

        if (!isGrabbed && isHighlighted && IsGrabbing())
        {
            Grab(currentCol);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (HasTag(col.gameObject))
        {
            currentCol = col;
            Debug.Log("OnTriggerEnter");
            Debug.Log(HasTag(col.gameObject));
            if (!isGrabbed && IsGrabbing())
            {
                Grab(col);
            }


            Highlight(true);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject == collidedWith)
        {
            currentCol = null;
            Release();
        }

        Highlight(false);
    }

    private void Grab(Collider col)
    {
        collidedWith = col.gameObject;
        isGrabbed = true;
        onGrab.Invoke();
        Debug.Log("Colliding with: " + col.gameObject.name);
    }

    private void Hold()
    {
        onHold.Invoke();
    }

    private void Release()
    {
        Debug.Log("Stopped colliding with: " + collidedWith.gameObject.name);
        collidedWith = null;
        isGrabbed = false;
        onRelease.Invoke();
    }

    private bool IsGrabbing()
    {
        return InputController.VRInput.Grab();
    }

    private bool HasTag(GameObject hasTag)
    {
        foreach (string t in tags)
        {
            if (hasTag.tag == t)
                return true;
        }

        return false;
    }

    private void Highlight(bool hightlight)
    {
        Material m = GetComponent<MeshRenderer>().material;
        m.shader = hightlight ? Shader.Find("Outlined/Silhouetted Diffuse") : startShader;
        isHighlighted = hightlight;
    }
}
