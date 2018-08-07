using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "MovingPlatform")
        {
            transform.parent = collision.collider.transform;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.tag == "MovingPlatform")
        {
            transform.parent = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Respawn":
                GameObject.Find("GameManager").GetComponent<GameManager>().Reset();
                break;
            case "Goal":
                GameObject.Find("GameManager").GetComponent<GameManager>().LevelFinished();
                break;
        }
    }
}
