using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public float position;
    public float positionCounter = 0.1f;
    public float positionIncrement = 0.01f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		position = Mathf.Sin(positionCounter)/10;
        Vector3 newPosition = new Vector3(position, 0, 0);
        transform.position += newPosition;
        positionCounter += positionIncrement;
	}


}
