using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorChecker : MonoBehaviour, IInteractable {

    public string onTouchError;
    public string onTouchStopError;
    public string onGrabError;
    public string onReleaseError;
    private AudioSource audioSource;
    public AudioClip clip;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f;

    }

    public void OnGrab(Transform hand)
    {
        if(onGrabError.Length > 0)
        {
            ErrorRegistry.Register(onGrabError);
            audioSource.PlayOneShot(clip, 0.5f);
        }
            
    }

    public void OnRelease(Transform hand)
    {
        if (onReleaseError.Length > 0)
        {
            ErrorRegistry.Register(onGrabError);
            
        }
            
    }

    public void OnTouch(Transform hand)
    {
        if (onTouchError.Length > 0)
        {
            ErrorRegistry.Register(onGrabError);
            audioSource.PlayOneShot(clip, 0.5f);
        }
            
    }

    public void OnTouchStop(Transform hand)
    {
        if (onTouchStopError.Length > 0)
        {
            ErrorRegistry.Register(onGrabError);
            audioSource.PlayOneShot(clip, 0.5f);
        }
            
    }
}
