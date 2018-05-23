using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {

    void OnGrab(Transform hand);
    void OnRelease(Transform hand);
    void OnTouch(Transform hand);
    void OnTouchStop(Transform hand);

}
