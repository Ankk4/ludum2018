using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class Player : MonoBehaviour {

    [SerializeField]
    private GameObject m_player;
    [SerializeField]
    private Text m_turnTimeText;
    [SerializeField]
    private Rigidbody m_rb;
    [SerializeField]
    private float m_speed;
    [SerializeField]
    private float m_jumpForce;
    [SerializeField]
    private float m_turnTime;

    public Boundary m_boundary;

    private float m_turnTimer;
    private bool m_turnActive;
    private Quaternion startinRotation;
    private bool m_canJump; 

    // Use this for initialization
    void Start ()
    {
        this.m_turnTimer = m_turnTime;
        this.m_turnActive = true;
        this.startinRotation = m_rb.rotation;
        this.m_canJump = true;
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    void FixedUpdate()
    {
        // Can jump?
        float rayDistance = GetComponent<Collider>().bounds.extents.y + 0.1f;
        Ray ray = new Ray();
        ray.origin = GetComponent<Collider>().bounds.center;
        ray.direction = Vector3.down;

        if (Physics.Raycast(ray, rayDistance))
        {
            m_canJump = true;
        }
        else
        {
            m_canJump = false;
        }

        // Movement
        if (m_canJump)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            m_rb.velocity = new Vector3(movement.x * m_speed, m_rb.velocity.y, movement.z * m_speed);
        }
        else
        {
            m_rb.velocity = new Vector3(m_rb.velocity.x, m_rb.velocity.y, m_rb.velocity.z);
        }

        // Bounding
        /*m_rb.position = new Vector3
        (
            Mathf.Clamp(m_rb.position.x, m_boundary.xMin, m_boundary.xMax),
            m_rb.position.y,
            Mathf.Clamp(m_rb.position.z, m_boundary.zMin, m_boundary.zMax)
        );*/

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (m_canJump)
            {
                m_rb.AddForce(new Vector3(0, m_jumpForce, 0), ForceMode.Impulse);
            }
        }

        if (Input.GetKey(KeyCode.Z) && m_turnActive == false)
        {
            TurnStart();
        }

        if (Input.GetKey(KeyCode.R))
        {
            Reset();
        }

        if (m_turnActive)
        {
            m_turnTimer -= Time.deltaTime;
            m_turnTimeText.text = m_turnTimer.ToString("F2");
            if (m_turnTimer < 0)
            {
                TurnEnd();
            }
        }
    }

    void TurnEnd()
    {
        m_rb.isKinematic = true;
        m_turnActive = false;
        m_turnTimeText.text = "00:00";
    }

    void TurnStart()
    {
        m_rb.isKinematic = false;
        m_turnActive = true;
        m_turnTimer = m_turnTime;
    }

    private void Reset()
    {
        m_rb.isKinematic = false;
        m_turnActive = true;
        m_turnTimer = m_turnTime;

        m_rb.position = new Vector3(0f, 2.7f, -7f);
        m_rb.velocity = Vector3.zero;
        m_rb.rotation = startinRotation;
    }
}
