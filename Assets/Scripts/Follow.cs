using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public GameObject parent;
    private Vector3 offset;

	// Use this for initialization
	void Start () {
		transform.position = parent.transform.position;
        offset = new Vector3(0, 0.32f);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = parent.transform.position + offset;
    }
}
