using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trachea : MonoBehaviour {

    public GameObject dropPrefab;
    public float dropYPos;
    public GameObject marker;
    public GameObject vocalCords;
    public float allowedDistance;
    public TextMesh finishText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (IsObjectBetweenMarkers(marker) && !finishText.gameObject.activeSelf)
        {
            finishText.gameObject.SetActive(true);
            Debug.Log("Tube in position!");
        }
        else if (finishText.gameObject.activeSelf)
        {
            finishText.gameObject.SetActive(false);
        }
	}

    private bool IsObjectBetweenMarkers(GameObject g)
    {
        return Vector3.Distance(g.transform.position, vocalCords.transform.position) < allowedDistance;
    }

    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("Touched trachea!");
        Vector3 dropPos = col.contacts[0].point;
        dropPos.y += dropYPos;
        SpawnDrop(dropPos);
    }

    private void SpawnDrop(Vector3 pos)
    {
        GameObject drop = GameObject.Instantiate(dropPrefab);
        drop.transform.position = pos;
    }
}
