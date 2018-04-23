using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSlowPlatform : MonoBehaviour {

    public GameObject gm;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    
    void OnCollisionStay(Collision other) {
        //Debug.Log("wadap");
        if(other.collider.tag == "Player") {
            gm.GetComponent<GameManager>().ModifyVelocityModifier(0.5f);
            Debug.Log("ass");
        } else {
            gm.GetComponent<GameManager>().ModifyVelocityModifier(1);
        }
    }
    /*
    void OnTriggerStay(Collider other) {
        //Debug.Log("wadap");
        if(other.tag == "Player") {
            gm.GetComponent<GameManager>().ModifyVelocityModifier(0.5f);
            Debug.Log("ass");
        } else {
            gm.GetComponent<GameManager>().ModifyVelocityModifier(1);
        }
    }*/
}
