using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public Camera parent;
    private Vector3 offset;

	// Use this for initialization
	void Start () {
        parent = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        transform.position = parent.transform.position;
        offset = new Vector3(0, 0.32f);
	}

    void Awake() {
        parent = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        transform.position = parent.transform.position;
        offset = new Vector3(0,0.32f);
    }
	
	// Update is called once per frame
	void Update () {
       // transform.position = parent.transform.position + offset;
    }
}
