using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    private bool isCollapsed;
    public string sceneNameToLoad;
    private Vector2 startPos;
    public float animateSpeed;
    public GameObject arrow;
    public GameObject menu;
    private float menuWidth;
    public Text errorDisplay;

	// Use this for initialization
	void Start ()
    {
        menuWidth = menu.GetComponent<RectTransform>().sizeDelta.x;
        startPos = menu.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update ()
    {
        menu.transform.localPosition = Vector2.MoveTowards(menu.transform.localPosition, isCollapsed ? new Vector2(startPos.x - menuWidth, startPos.y) : startPos, animateSpeed *Time.deltaTime);

        errorDisplay.text = ErrorRegistry.GetErrorString();
	}

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }

    public void Collapse()
    {
        Quaternion newArrowRot = arrow.transform.localRotation;
        newArrowRot.z *= -1;
        arrow.transform.localRotation = newArrowRot;
        isCollapsed = !isCollapsed;
    }
}
