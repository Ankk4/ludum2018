using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformAction : MonoBehaviour {

    public float position;
    public float positionCounter = 0.1f;
    public float positionIncrement = 0.01f;
    public float speed = 1;
    private int direction = 1;
    public GameObject gm;
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        GameManager.GameState gs = gm.GetComponent<GameManager>().CurrentGameState;
        if(gs == GameManager.GameState.action) {
            position = Mathf.Sin(positionCounter) / 10;
            Vector3 newPosition = new Vector3(position,0,0);
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
