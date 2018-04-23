using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject gameManager;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.tag == "MovingPlatform")
        {
            Debug.Log("nigga");
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
        switch (other.tag)
        {
            case "Respawn":
                gameManager.GetComponent<GameManager>().Reset();
                break;
            case "Goal":
                gameManager.GetComponent<GameManager>().LevelFinished();
                break;
        }
    }
}

