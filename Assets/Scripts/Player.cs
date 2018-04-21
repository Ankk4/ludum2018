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
    private GameObject player;
    [SerializeField]
    private Text turnTimeText;
    [SerializeField]
    private Text inputText;
    [SerializeField]
    private Text turnText;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float turnTime;

    public Boundary boundary;

    public enum GameState
    {
        menu,
        action,
        pause,
        simulation
    }

    private GameState currentGameState;

    private float turnTimer;
    private bool turnActive;
    private Quaternion startinRotation;
    private Vector3 momentum;
    private bool canJump;
    private int recordIterator;

    //private IDictionary<, int> dict = new Dictionary<string, int>();
    //private Vector3[] velocityRecords;
    private ArrayList positionRecord;

    private Vector3 startP, endP;
    private float simulationPointDistance;
    private int currentSimPoint;

    // Use this for initialization
    void Start ()
    {
        this.turnTimer = turnTime;
        this.turnActive = true;
        this.startinRotation = this.rb.rotation;
        this.canJump = true;
        //this.velocityRecords = new Vector3[1000];
        this.positionRecord = new ArrayList();
        this.recordIterator = 0;
        this.currentSimPoint = 0;
        this.currentGameState = GameState.action;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (turnActive == false && currentGameState == GameState.action)
            {
                currentGameState = GameState.pause;
                turnText.text = "Pause Turn";
            }
            else if (currentGameState == GameState.pause)
            {
                currentGameState = GameState.simulation;
                startP = (Vector3)positionRecord[0];
                endP = (Vector3)positionRecord[1];
                turnText.text = "Simulation Turn";
                ResetForSimulation();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            DebugReset();
        }
    }

    void FixedUpdate()
    {
        if (currentGameState == GameState.action)
        {
            HandleActionTurn();
            turnText.text = "Action Turn";
        }
        else if (currentGameState == GameState.pause)
        {

        }
        else if (currentGameState == GameState.simulation)
        {
            rb.position = Vector3.Lerp(startP, endP, Time.deltaTime);
            if (currentSimPoint + 1 < positionRecord.Count - 1)
            {
                currentSimPoint++;
                SetNextSimulationPoint(currentSimPoint);
            }
        }
    }

    private void HandleActionTurn()
    {
        Move();
        
        // record
        if (turnActive)
        {
            //velocityRecords[recordIterator] = rb.velocity;
            //recordIterator++;
            positionRecord.Add(rb.position);
        }

        // Bounding
        /*rb.position = new Vector3
        (
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            rb.position.y,
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
        );*/


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump)
            {
                rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            }
        }

        /*if (Input.GetKey(KeyCode.Z) && turnActive == false)
        {
            ActionTurnStart();
        }*/

        if (turnActive)
        {
            turnTimer -= Time.deltaTime;
            turnTimeText.text = turnTimer.ToString("F2");
            if (turnTimer < 0)
            {
                ActionTurnEnd();
            }
        }
    }

    private void Move()
    {
        // Can jump?
        float rayDistance = GetComponent<Collider>().bounds.extents.y + 0.1f;
        Ray ray = new Ray();
        ray.origin = GetComponent<Collider>().bounds.center;
        ray.direction = Vector3.down;

        if (Physics.Raycast(ray, rayDistance))
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }

        // Movement
        if (canJump)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            rb.velocity = new Vector3(movement.x * speed, rb.velocity.y, movement.z * speed);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        }
    }

    private void ActionTurnEnd()
    {
        rb.isKinematic = true;
        turnActive = false;
        turnTimeText.text = "00:00";
    }

    private void ActionTurnStart()
    {
        momentum = rb.velocity;
        turnTimeText.text = "00:00";
        rb.isKinematic = false;
        turnActive = true;
        turnTimer = turnTime;
    }

    private void DebugReset()
    {
        rb.velocity = momentum;
        rb.isKinematic = false;
        turnActive = true;
        turnTimer = turnTime;

        rb.position = new Vector3(0f, 2.7f, -7f);
        rb.velocity = Vector3.zero;
        rb.rotation = startinRotation;
    }

    private void ResetForSimulation()
    {
        rb.isKinematic = false;
        turnTimer = turnTime;
        turnTimeText.text = turnTimer.ToString("F2");

        rb.position = new Vector3(0f, 2.7f, -7f);
        rb.velocity = Vector3.zero;
        rb.rotation = startinRotation;
    }

    private void SetNextSimulationPoint(int currentPoint)
    {
        startP = (Vector3) positionRecord[currentPoint];
        endP = (Vector3) positionRecord[currentPoint + 1];
        simulationPointDistance = Vector3.Distance(startP, endP);
    }

    
    void OnCollisionEnter(Collision collision) {
        if(collision.collider.gameObject.tag == "ground") {
            Debug.Log("Bob");
            transform.parent = collision.collider.transform;
        }
    }
    void OnCollisionExit(Collision collision) {
        if(collision.collider.gameObject.tag == "ground") {
            transform.parent = null;
        }
    }
    
}
