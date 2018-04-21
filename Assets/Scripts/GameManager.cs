using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private Text turnTimeText;
    [SerializeField]
    private Text turnText;
    [SerializeField]
    private Rigidbody player_rb;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float turnTime;
    [SerializeField]
    private Vector3 startingPoint;

    private float turnTimer;

    public enum GameState
    {
        menu,
        action,
        pause,
        simulation
    }
    private GameState currentGameState;

    private Quaternion startinRotation;
    private Vector3 momentum;
    private bool canJump;

    private int recordIterator;
    private Vector3[] positionRecord;
    private Vector3 startP, endP;
    private int currentSimPoint;
    private bool simulationOver;
    private float journeyLength;

    void Start()
    {
        this.turnTimer = turnTime;
        this.startinRotation = this.player_rb.rotation;
        this.canJump = true;
        this.positionRecord = new Vector3[1000];
        this.recordIterator = 0;
        this.currentSimPoint = 0;
        this.currentGameState = GameState.action;
        this.turnText.text = "Action Turn";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && currentGameState == GameState.pause)
        {
            currentGameState = GameState.simulation;
            startP = positionRecord[0];
            endP = positionRecord[1];
            journeyLength = Vector3.Distance(startP, endP);
            turnText.text = "Simulation Turn";
            ResetForSimulation();
        }
        else if (Input.GetKeyDown(KeyCode.Z) && currentGameState == GameState.simulation && simulationOver)
        {
            currentGameState = GameState.action;
            ActionTurnStart();
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
        }
        else if (currentGameState == GameState.pause)
        {
        }
        else if (currentGameState == GameState.simulation)
        {
            HandleSimulationTurn();
        }
    }

    private void HandleSimulationTurn()
    {
        // SIMULATION OVER?
        if (currentSimPoint == recordIterator)
        {
            player_rb.isKinematic = true;
            simulationOver = true;
        }
        else
        {
            float distCovered = (Time.time - turnTime) * speed;
            float fracJourney = distCovered / journeyLength;
            player_rb.position = Vector3.Lerp(startP, endP, Time.deltaTime);

            // DO WE NEED NEW END POS?
            if (fracJourney >= 1f)
            {
                currentSimPoint++;
                startP = positionRecord[currentSimPoint];
                endP = positionRecord[currentSimPoint + 1];
                journeyLength = Vector3.Distance(startP, endP);
                currentSimPoint++;
            }

            turnTimer -= Time.deltaTime;
            turnTimeText.text = turnTimer.ToString("F2");
        }
    }

    private void HandleActionTurn()
    {
        Move();

        // record
        positionRecord[recordIterator] = player_rb.position;
        recordIterator++;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump)
            {
                player_rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            }
        }

        turnTimer -= Time.deltaTime;
        turnTimeText.text = turnTimer.ToString("F2");
        if (turnTimer < 0)
        {
            ActionTurnEnd();
        }
    }

    private void Move()
    {
        // Can jump?
        float rayDistance = player_rb.GetComponent<Collider>().bounds.extents.y + 0.1f;
        Ray ray = new Ray();
        ray.origin = player_rb.GetComponent<Collider>().bounds.center;
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
            player_rb.velocity = new Vector3(movement.x * speed, player_rb.velocity.y, movement.z * speed);
        }
        else
        {
            player_rb.velocity = new Vector3(player_rb.velocity.x, player_rb.velocity.y, player_rb.velocity.z);
        }
    }

    private void ActionTurnEnd()
    {
        momentum = player_rb.velocity;
        player_rb.isKinematic = true;

        currentGameState = GameState.pause;
        turnText.text = "Pause Turn";
        turnTimeText.text = "00:00";
    }

    private void ActionTurnStart()
    {
        turnText.text = "Action Turn";
        player_rb.velocity = momentum;
        player_rb.isKinematic = false;
        turnTimer = turnTime;
        recordIterator = 0;
        currentSimPoint = 0;
    }

    private void DebugReset()
    {
        player_rb.velocity = momentum;
        player_rb.isKinematic = false;
        turnTimer = turnTime;

        player_rb.position = startingPoint;
        player_rb.velocity = Vector3.zero;
        player_rb.rotation = startinRotation;

        turnTimer = turnTime;
        startinRotation = this.player_rb.rotation;
        canJump = true;
        positionRecord = new Vector3[1000];
        currentSimPoint = 0;
        currentGameState = GameState.action;
    }

    private void ResetForSimulation()
    {
        player_rb.isKinematic = false;
        simulationOver = false;
        turnTimer = turnTime;

        player_rb.velocity = Vector3.zero;
        player_rb.rotation = startinRotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "ground")
        {
            transform.parent = collision.collider.transform;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.tag == "ground")
        {
            transform.parent = null;
        }
    }

}
