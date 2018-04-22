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

    private Quaternion startingRotation;
    private Vector3 momentum;
    private bool canJump;

    private int recordIterator;
    private int currentSimPoint;
    private Vector3[] velocityRecord;

    void Start()
    {
        this.turnTimer = turnTime;
        this.startingRotation = this.player_rb.rotation;
        this.canJump = true;
        this.velocityRecord = new Vector3[1000];
        this.recordIterator = 0;
        this.currentSimPoint = 0;
        this.currentGameState = GameState.action;
        this.turnText.text = "Action Turn";
        player_rb.isKinematic = true;
        player_rb.GetComponent<Collider>().enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            switch (currentGameState)
            {
                case GameState.pause:
                    currentGameState = GameState.simulation;
                    turnText.text = "Simulation Turn";

                    // Activate player model for simulation. 
                    player_rb.isKinematic = false;
                    player_rb.GetComponent<Collider>().enabled = true;
                    turnTimer = turnTime;
                    break;

                case GameState.readyForAction:
                    currentGameState = GameState.action;
                    ActionTurnStart();
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            DebugReset();
        }
    }

    void FixedUpdate()
    {
        switch (currentGameState)
        {
            case GameState.action:
                HandleActionTurn();
                break;

            case GameState.pause:
                // do some pausy things
                break;

            case GameState.simulation:
                HandleSimulationTurn();
                break;

            case GameState.readyForAction:
                // do some readyforactiony things
                break;
        }
    }

    private void HandleSimulationTurn()
    {
        player_rb.velocity = velocityRecord[currentSimPoint];
        currentSimPoint++;

        // Check if Simulation is done
        if (currentSimPoint == recordIterator)
        {
            player_rb.isKinematic = true;
            currentGameState = GameState.readyForAction;

            turnText.text = "Press z to start action turn";
            turnTimeText.text = "00.00";
            turnTimer = turnTime;
        }
    }

    private void HandleActionTurn()
    {
        // Player input
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump)
            {
                ghost_rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            }
        }

        // Record velocity EVERY FRAME!!
        velocityRecord[recordIterator] = ghost_rb.velocity;
        recordIterator++;

        // Update turn timer
        turnTimer -= Time.deltaTime;
        turnTimeText.text = turnTimer.ToString("F2");

        // Turn timer ran out --> Action Turn over. 
        if (turnTimer < 0)
        {
            // Set everything to be kinematic and no collide.
            momentum = ghost_rb.velocity;
            player_rb.isKinematic = true;
            player_rb.GetComponent<Collider>().enabled = true;

            ghost_rb.isKinematic = true;
            ghost_rb.GetComponent<Collider>().enabled = true;

            currentGameState = GameState.pause;
            turnText.text = "Pause Turn";
            turnTimeText.text = "00:00";
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

    private void ActionTurnStart()
    {
        // Set ghost to player position and unlock movement. 
        turnText.text = "Action Turn";
        ghost_rb.velocity = momentum;
        ghost_rb.position = player_rb.position;
        ghost_rb.isKinematic = false;
        ghost_rb.GetComponent<Collider>().enabled = true;

        // Set player to stay put 
        player_rb.isKinematic = true;
        player_rb.GetComponent<Collider>().enabled = false;
        recordIterator = 0;
        currentSimPoint = 0;
    }

    private void DebugReset()
    {
        // Reset Game
        currentGameState = GameState.action;
        turnTimer = turnTime;

        // Reset movement
        canJump = true;
        player_rb.isKinematic = false;
        player_rb.position = startingPoint;
        player_rb.velocity = Vector3.zero;
        player_rb.rotation = startingRotation;
        ghost_rb.position = player_rb.position;
        ghost_rb.velocity = player_rb.velocity;
        ghost_rb.rotation = player_rb.rotation;

        // Reset recording
        velocityRecord = new Vector3[1000];
        currentSimPoint = 0;
    }
}
