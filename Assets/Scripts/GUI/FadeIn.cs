using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour {

    private float fadeDuration = 2f;

    void Start ()
    {
        FadeToBlack();
        FadeFromBlack();
	}
	
    private void FadeToBlack()
    {
        SteamVR_Fade.Start(Color.black, 0f);        
    }

    private void FadeFromBlack()
    {
        SteamVR_Fade.Start(Color.clear, fadeDuration);
    }
}
