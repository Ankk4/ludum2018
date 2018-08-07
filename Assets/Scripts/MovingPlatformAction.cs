using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformAction : MonoBehaviour {

    public float position;

    //X=0, Y=1, Z=2
    public int axel;
    public float positionCounter = 0.1f;
    public float positionIncrement = 0.01f;
    private Vector3 newPosition;

    // Use this for initialization
    void Start ()
    {        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (GameManager.CurrentGameState == GameManager.GameState.action)
        {
            switch(axel)
            {
                case 0:
                    position = Mathf.Sin(positionCounter) / 10;
                    newPosition = new Vector3(position,0,0);
                    break;
                case 1:
                    position = Mathf.Sin(positionCounter) / 10;
                    newPosition = new Vector3(0,position,0);
                    break;
                case 2:
                    position = Mathf.Sin(positionCounter) / 10;
                    newPosition = new Vector3(0,0,position);
                    break;
                default:
                    break;
            }
            
            transform.position += newPosition;
            positionCounter -= positionIncrement;
        }
		        
        //transform.Translate(Vector3.forward * speed * direction * Time.deltaTime);
	}
    /*
    void OnTriggerEnter(Collider other) {
        Debug.Log("ontrigger");
        if(other.tag == "target") {
            Debug.Log("target");
            direction *= -1;
        }
    }*/


}
