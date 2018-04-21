using UnityEngine;

public class Player : MonoBehaviour {

    void OnCollisionEnter(Collision collision) {
        if(collision.collider.gameObject.tag == "ground") {
            transform.parent = collision.collider.transform;
        }
    }

    void OnCollisionExit(Collision collision) {
        if(collision.collider.gameObject.tag == "ground") {
            transform.parent = null;
        }
    }

}

