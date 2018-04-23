using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private Text turnTimeText;
    [SerializeField]
    private Text turnText;
    [SerializeField]
    public Rigidbody player_rb;
    [SerializeField]
    public Rigidbody ghost_rb;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float turnTime;
    [SerializeField]
    private Vector3 startingPoint;
    [SerializeField]
    private GameObject pauseMenu;
    private Toggle toggle;
    public ToggleGroup upgrades;

    private float lastActionTime;
    private float turnTimer;

    public enum GameState
    {
        menu,
        action,
        pause,
        simulation,
        readyForAction
    }
    private GameState currentGameState;
    public GameState CurrentGameState
    {
        get { return currentGameState; }
    }

    private Quaternion startingRotation;
    private Vector3 momentum;
    private bool canJump;

    private int recordIterator;
    private int currentSimPoint;
    private Vector3[] velocityRecord;
    private float upgradeVelocity, velocityModifier;
    private bool playerSolid, ghostSolid, spawnToGhost;

    void Start()
    {
        this.turnTime = 5.0f;
        this.lastActionTime = 5.0f;
        this.turnTimer = turnTime;
        this.startingRotation = this.player_rb.rotation;
        this.startingPoint = this.player_rb.transform.position;
        this.ghost_rb.transform.position = player_rb.transform.position;
        this.canJump = true;
        this.velocityRecord = new Vector3[1000];
        this.recordIterator = 0;
        this.currentSimPoint = 0;
        this.currentGameState = GameState.action;
        this.turnText.text = "Action Turn";
        this.player_rb.isKinematic = true;
        this.player_rb.GetComponent<Collider>().enabled = false;
        this.playerSolid = false;
        this.ghostSolid = false;
        this.spawnToGhost = false;
        velocityModifier = 1;
        //toggle = pauseMenu.transform.Find("SpeedUp").gameObject.GetComponent<Toggle>();
        //toggle.isOn = false;
        //upgradeVelocity = 1;
        //GameObject.FindGameObjectWithTag("music").GetComponent<Music>().PlayMusic();
    }

    void Update()
    {
        if (Input.GetButton("Advance"))
        {
            switch (currentGameState)
            {
                case GameState.pause:
                    currentGameState = GameState.simulation;
                    turnText.text = "Simulation Turn";

                    // Activate player model for simulation. 
                    player_rb.isKinematic = false;
                    player_rb.GetComponent<Collider>().enabled = true;
                    ghost_rb.isKinematic = true;
                    ghost_rb.GetComponent<Collider>().enabled = false;
                    turnTimer = lastActionTime;
                    break;

                case GameState.readyForAction:
                    currentGameState = GameState.action;
                    ActionTurnStart();
                    break;
            }
        }

        if (Input.GetButton("Reset"))
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
                HandlePauseTurn();
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
        pauseMenu.SetActive(false);
        // Update turn timer
        turnTimer -= Time.deltaTime;
        turnTimeText.text = turnTimer.ToString("F2");

        player_rb.velocity = velocityRecord[currentSimPoint] * upgradeVelocity * velocityModifier;
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
        if(playerSolid) {
            if((ghost_rb.transform.position.x - player_rb.transform.position.x > 1) || (ghost_rb.transform.position.y - player_rb.transform.position.y > 1) || (ghost_rb.transform.position.z - player_rb.transform.position.z > 1)) {
                player_rb.GetComponent<Collider>().enabled = true;
            }
        }
        
        // Player input
        Move();
        if (Input.GetButton("Jump"))
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

            ResetUpgrades();
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
            ghost_rb.velocity = new Vector3(ghost_rb.velocity.x, ghost_rb.velocity.y, ghost_rb.velocity.z);
        }
    }

    private void ActionTurnStart()
    {

        //disable used upgrade
        //toggle = upgrades.ActiveToggles().FirstOrDefault();
        if(toggle.isOn)
            toggle.interactable = false;

        lastActionTime = turnTime;
        // Set ghost to player position and unlock movement. 
        turnText.text = "Action Turn";
        ghost_rb.velocity = momentum;

        if(!spawnToGhost)
            ghost_rb.position = player_rb.position;



        ghost_rb.isKinematic = false;
        ghost_rb.GetComponent<Collider>().enabled = true;

        // Set player to stay put 
        player_rb.isKinematic = true;

        
        player_rb.GetComponent<Collider>().enabled = false;

        recordIterator = 0;
        currentSimPoint = 0;
    }

    private void HandlePauseTurn() {       
        pauseMenu.SetActive(true);
        
    }

    private void DebugReset()
    {
        // Reset Game
        currentGameState = GameState.action;
        this.turnText.text = "Action Turn";
        turnTimer = turnTime;

        // Reset movement
        canJump = true;
        player_rb.position = startingPoint;
        player_rb.rotation = startingRotation;
        player_rb.velocity = Vector3.zero;
        player_rb.isKinematic = true;
        player_rb.GetComponent<Collider>().enabled = false;

        ghost_rb.position = player_rb.position;
        ghost_rb.velocity = Vector3.zero;
        ghost_rb.rotation = player_rb.rotation;
        ghost_rb.isKinematic = false;
        ghost_rb.GetComponent<Collider>().enabled = true;

        ResetUpgrades();
        turnTime = 5.0f;

        //activate upgrades
        toggle = pauseMenu.transform.Find("SpeedUp").gameObject.GetComponent<Toggle>();
        toggle.interactable = true;
        toggle = pauseMenu.transform.Find("MoreTime").gameObject.GetComponent<Toggle>();
        toggle.interactable = true;
        toggle = pauseMenu.transform.Find("KillMomentum").gameObject.GetComponent<Toggle>();
        toggle.interactable = true;
        toggle = pauseMenu.transform.Find("SimulationVelocity").gameObject.GetComponent<Toggle>();
        toggle.interactable = true;
        toggle = pauseMenu.transform.Find("SolidPlayer").gameObject.GetComponent<Toggle>();
        toggle.interactable = true;
        toggle = pauseMenu.transform.Find("SpawnToGhost").gameObject.GetComponent<Toggle>();
        toggle.interactable = true;

        // Reset recording
        velocityRecord = new Vector3[1000];
        currentSimPoint = 0;
        recordIterator = 0;

        //hide pausemenu
        pauseMenu.SetActive(false);
    }

    public void Reset()
    {
        // Reset Game
        currentGameState = GameState.action;
        this.turnText.text = "Action Turn";
        turnTime = 5.0f;
        turnTimer = turnTime;

        // Reset movement
        canJump = true;
        player_rb.position = startingPoint;
        player_rb.rotation = startingRotation;
        player_rb.velocity = Vector3.zero;
        player_rb.isKinematic = true;
        player_rb.GetComponent<Collider>().enabled = false;

        ghost_rb.position = player_rb.position;
        ghost_rb.velocity = Vector3.zero;
        ghost_rb.rotation = player_rb.rotation;
        ghost_rb.isKinematic = false;
        ghost_rb.GetComponent<Collider>().enabled = true;

        ResetUpgrades();
        
        //activate upgrades
        toggle = pauseMenu.transform.Find("SpeedUp").gameObject.GetComponent<Toggle>();
        toggle.interactable = true;
        toggle = pauseMenu.transform.Find("MoreTime").gameObject.GetComponent<Toggle>();
        toggle.interactable = true;
        toggle = pauseMenu.transform.Find("KillMomentum").gameObject.GetComponent<Toggle>();
        toggle.interactable = true;
        toggle = pauseMenu.transform.Find("SimulationVelocity").gameObject.GetComponent<Toggle>();
        toggle.interactable = true;
        toggle = pauseMenu.transform.Find("SolidPlayer").gameObject.GetComponent<Toggle>();
        toggle.interactable = true;
        toggle = pauseMenu.transform.Find("SpawnToGhost").gameObject.GetComponent<Toggle>();
        toggle.interactable = true;

        // Reset recording
        velocityRecord = new Vector3[1000];
        currentSimPoint = 0;
        recordIterator = 0;

        //hide pausemenu
        pauseMenu.SetActive(false);
    }

    private void ResetUpgrades()
    {
        //toggle = upgrades.ActiveToggles().FirstOrDefault();
        //toggle.isOn = false;
        //upgrades.SetAllTogglesOff();

        //Reset all player attributes to normal
        turnTime = 5.0f;
        upgradeVelocity = 1;
        playerSolid = false;
        ghostSolid = false;
        spawnToGhost = false;
        velocityModifier = 1;

        //Untoggle the toggles
        toggle = pauseMenu.transform.Find("SpeedUp").gameObject.GetComponent<Toggle>();
        toggle.isOn = false;
        toggle = pauseMenu.transform.Find("MoreTime").gameObject.GetComponent<Toggle>();
        toggle.isOn = false;
        toggle = pauseMenu.transform.Find("KillMomentum").gameObject.GetComponent<Toggle>();
        toggle.isOn = false;
        toggle = pauseMenu.transform.Find("SimulationVelocity").gameObject.GetComponent<Toggle>();
        toggle.isOn = false;
        toggle = pauseMenu.transform.Find("SolidPlayer").gameObject.GetComponent<Toggle>();
        toggle.isOn = false;
        toggle = pauseMenu.transform.Find("SpawnToGhost").gameObject.GetComponent<Toggle>();
        toggle.isOn = false;

        
    }

    public void SpeedUpgrade() {
        toggle = pauseMenu.transform.Find("SpeedUp").gameObject.GetComponent<Toggle>();   
        if(toggle.isOn) {
            Debug.Log(toggle.name);            
            speed = 10;
        } else {
            speed = 6;
        }
    }

    public void MoreTime()
    {
        toggle = pauseMenu.transform.Find("MoreTime").gameObject.GetComponent<Toggle>();

        if(toggle.isOn)
        {
            Debug.Log(toggle.name);
            turnTime = 10;            
        } else
        {
            turnTime = 5;
        }
    }

    public void KillMomentum()
    {
        toggle = pauseMenu.transform.Find("KillMomentum").gameObject.GetComponent<Toggle>();
        if(toggle.isOn)
        {
            Debug.Log(toggle.name);
            momentum = new Vector3(0,0,0);
        }
    }

    public void SimulationVelocity()
    {
        toggle = pauseMenu.transform.Find("SimulationVelocity").gameObject.GetComponent<Toggle>();
        if(toggle.isOn)
        {
            upgradeVelocity = 1.5f;                           
        }
        else
        {
            upgradeVelocity = 1; 
        }
    }

    public void SolidPlayer()
    {
        toggle = pauseMenu.transform.Find("SolidPlayer").gameObject.GetComponent<Toggle>();
        if(toggle.isOn)
        {
            playerSolid = true;
        }
        else
        {
            playerSolid = false;
        }
    }

    public void SpawnToGhost() {
        toggle = pauseMenu.transform.Find("SpawnToGhost").gameObject.GetComponent<Toggle>();

        if(toggle.isOn) {
            spawnToGhost = true;
        } else {
            spawnToGhost = false;
        }
    }

    public void LevelFinished ()
    {
        SceneManager.LoadScene("level2");
        //Reset();
    }

    public void ModifyVelocityModifier(float modifier) {
        velocityModifier = modifier;
    }
}
