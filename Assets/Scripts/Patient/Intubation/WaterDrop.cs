using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour {

    public float speed;
    public Projector projector;
    public float stopAt;

	// Update is called once per frame
	void Update () {

        //Lerp fov value closer to 0
        projector.fieldOfView = Mathf.Lerp(projector.fieldOfView, 0, speed * Time.deltaTime);
        
        if (IsWithinRange())
            Destroy(this.gameObject);
	}


    //Return if projector fov is smaller than or equal to value where animation needs to stop, return true
    private bool IsWithinRange()
    {
        return projector.fieldOfView <= stopAt;
    }
}
