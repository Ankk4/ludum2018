using UnityEngine;

public class Player : MonoBehaviour {

    void OnCollisionEnter(Collision collision) {
        if(collision.collider.gameObject.tag == "MovingPlatform") {
            transform.parent = collision.collider.transform;
        }
    }

    void OnCollisionExit(Collision collision) {
        if(collision.collider.gameObject.tag == "MovingPlatform") {
            transform.parent = null;
        }
    }

}

