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

    public Rigidbody ghost_rb;

    public enum GameState
    {
        menu,
        action,
        pause,
        simulation,
        readyForAction
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
        player_rb.isKinematic = true;
        player_rb.GetComponent<Collider>().enabled = false;
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
        else if (Input.GetKeyDown(KeyCode.Z) && currentGameState == GameState.readyForAction && simulationOver)
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
            currentGameState = GameState.readyForAction;
            ReadyActionTurnStart();
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
        positionRecord[recordIterator] = ghost_rb.position;
        recordIterator++;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump)
            {
                ghost_rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
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
        float rayDistance = ghost_rb.GetComponent<Collider>().bounds.extents.y + 0.1f;
        Ray ray = new Ray();
        ray.origin = ghost_rb.GetComponent<Collider>().bounds.center;
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
            ghost_rb.velocity = new Vector3(movement.x * speed, player_rb.velocity.y, movement.z * speed);
        }
        else
        {
            player_rb.velocity = new Vector3(player_rb.velocity.x, player_rb.velocity.y, player_rb.velocity.z);
        }
    }

    private void ActionTurnEnd()
    {
        momentum = ghost_rb.velocity;
        player_rb.isKinematic = false;
        player_rb.GetComponent<Collider>().enabled = true;
        ghost_rb.isKinematic = true;
        ghost_rb.GetComponent<Collider>().enabled = false;

        currentGameState = GameState.pause;
        turnText.text = "Pause Turn";
        turnTimeText.text = "00:00";
    }

    private void ActionTurnStart()
    {
        turnText.text = "Action Turn";
        ghost_rb.velocity = momentum;        
        ghost_rb.isKinematic = false;
        ghost_rb.GetComponent<Collider>().enabled = true;
        player_rb.isKinematic = true;
        player_rb.GetComponent<Collider>().enabled = false;
        recordIterator = 0;
        currentSimPoint = 0;
    }

    private void ReadyActionTurnStart() {
        turnText.text = "Press z to start action turn";
        turnTimeText.text = "00.00";
        turnTimer = turnTime;
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


}
