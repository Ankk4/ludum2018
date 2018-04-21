using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    private GameObject m_player;
    [SerializeField]
    private Rigidbody m_rb;
    [SerializeField]
    private float m_speed;
    [SerializeField]
    private float m_jumpForce;
    private float m_jumpSpeed;

    private bool m_grounded;

    // Use this for initialization
    void Start ()
    {
        this.m_grounded = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_player.transform.position += Vector3.left * m_speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            m_player.transform.position += Vector3.right * m_speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            m_player.transform.position += Vector3.forward * m_speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            m_player.transform.position += Vector3.back * m_speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && m_grounded)
        {
            m_rb.velocity = new Vector3(0f, m_jumpForce, 0f);
            m_grounded = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("ground"))
        {
            Debug.Log("jee");
            m_grounded = true;
        }
    }
}
