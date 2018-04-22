using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject gameManager;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.tag == "MovingPlatform")
        {
            transform.parent = collision.collider.transform;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if(collision.collider.gameObject.tag == "MovingPlatform")
        {
            transform.parent = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Respawn")
        {
            gameManager.GetComponent<GameManager>().Reset();
        }
    }

}

