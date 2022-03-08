using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

/*
* TODO: 
* 1. (in edit mode test) add tests for testing time travel past the total elapsed time/beginning
* 2. fix assignment of player icons on time bar
*/

public class PlayerMovement : MonoBehaviour {

	// variables defining player values
	public CharacterController characterBody;
	public Camera cam;
	public GameObject cameraHolder;
	public Canvas UI;
	public Transform groundCheck;
	public LayerMask groundMask;
	public float mouseSensitivity = 100f;
	public int team; // 0 seeker 1 hider 
	private float speed = 5f;
	private float gravity = 40f;
	private float jumpPower = 3f;
	private float groundCheckRadius = 0.5f;
	private bool isGrounded = true;
	private Vector3 velocity;
	private float xRot = 0f;
	private Vector3 lastPos;
	private float ab1Cooldown = 0f;
	private float ab2Cooldown = 0f;
	private int timeJumpAmount = 100;

	// variables corresponding to UI
	public PauseManager pauseUI;
	public GameObject nametag;

    // variables corresponding to player Animations
	public Animator playerAnim;
	public Transform grabCheck;
	public LayerMask grabMask;
	private float grabCheckRadius = 1f;
	private bool damageWindow = false;

    // the photonView component that syncs with the network
	public PhotonView view;

	// Time control variables
	public TimeConn timeTravel;

    // variables corresponding to the gamestate
    public GameController game;
	public ParticleController particles;
	public PlayerHud hud;
	public PlayerMaterial material;

	// Start is called before the first frame update
	void Start() {
		DontDestroyOnLoad(this.gameObject);
		team = (int) GameController.Teams.Seeker; // Initialise to Seeker but call chang
		changeTeam(); // set the player's colour depending on their team
		if (!view.IsMine) {
			// destroy other player cameras and ui in local environment
			Destroy(cam.gameObject);
			Destroy(UI.gameObject);
			gameObject.layer = 7;
		} else {
			// destroy your own nametag
			Destroy(nametag);
			gameObject.tag = "Client";
		}
        PhotonNetwork.AutomaticallySyncScene = true;      // allow master client to move players from one scene to another
        Cursor.lockState = CursorLockMode.Locked;         // lock players cursor to center screen
        SceneManager.activeSceneChanged += onSceneChange; // link scenechange event to onscenechange
	}

	// onSceneChange is called by the SceneManager.activeSceneChanged event;
	void onSceneChange(Scene current, Scene next) {
		if (next.name == "GameScene") {
			game = FindObjectOfType<GameController>();
			timeTravel.connectToTimeLord();
			if (game == null) {
				Debug.Log("PlayerMovement scene change error: GameController is null");
			}
			ab1Cooldown = 15;
			ab2Cooldown = 15;
		}
	}

	// Update is called once per frame
	void Update() {
		// local keys only affect client's player
		if (!view.IsMine) return;

		if (SceneManager.GetActiveScene().name == "PreGameScene" ||
		(SceneManager.GetActiveScene().name == "GameScene" && !game.gameEnded)) {
			movementControl();
			cameraControl();
			UpdateCooldowns();
			KeyControl();
		}

		// Set the debug items and send to HUD to be displayed
		Hashtable debugItems = new Hashtable();
		Vector3 movementVector = transform.position - lastPos;
		float distTravelled = movementVector.magnitude / Time.deltaTime;
		debugItems.Add("Speed", distTravelled);
		debugItems.Add("Room", PhotonNetwork.CurrentRoom.Name);
		debugItems.Add("Sprint", Input.GetKey("left shift"));
		debugItems.Add("Grab", damageWindow);
		debugItems.Add("Ground", isGrounded);
		hud.SetDebugValues(debugItems);

		// update player cooldown displays
		float[] cooldownValues = new float[]{1.0f - (ab1Cooldown / 15.0f), 1.0f - (ab2Cooldown / 15.0f)};
		hud.SetCooldownValues(cooldownValues);

		bool canJumpForward = SceneManager.GetActiveScene().name == "GameScene" && ab1Cooldown <= 0.0f && 
							timeTravel.GetRealityTick() + (float) timeJumpAmount <= timeTravel.GetCurrentTick();
		bool canJumpBack = SceneManager.GetActiveScene().name == "GameScene" && ab2Cooldown <= 0.0f && 
							timeTravel.GetRealityTick() - (float) timeJumpAmount >= 0;
		hud.SetCanJump(canJumpForward, canJumpBack);

		// update pauseUI and cursor lock if game is ended
		if (SceneManager.GetActiveScene().name == "GameScene" && game.gameEnded)
		{
			pauseUI.isPaused = true;
			pauseUI.pauseMenuUI.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
		}
	}


	/*******************
	* Movement Control *
	********************/

    // handle movement axis inputs (wasd, arrowkeys, joystick)
	void movementControl() {
        lastPos = transform.position; // update lastPos from prev frame

        // only allow movement after game has started
		if (SceneManager.GetActiveScene().name == "GameScene" && game.gameStarted) {
            // sprint speed
			if (Input.GetKey("left shift")) {
				speed = 10f;
			} else {
				speed = 5f;
			}

            // get movement axis values
			float xMove = pauseUI.isPaused ? 0 : Input.GetAxis("Horizontal");
			float zMove = pauseUI.isPaused ? 0 : Input.GetAxis("Vertical");

            // check if player's GroundCheck intersects with any environment object
			isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

            // set and normalise movement vector
			Vector3 movement = (transform.right * xMove) + (transform.forward * zMove);
			if (movement.magnitude != 1 && movement.magnitude != 0) {
				movement /= movement.magnitude;
			}
            
			// transform according to movement vector
			characterBody.Move(movement * speed * Time.deltaTime);
		}

		// jump control
		if (Input.GetButtonDown("Jump") && isGrounded && !pauseUI.isPaused) {
			velocity.y += Mathf.Sqrt(jumpPower * 2f * gravity);
		}

		// gravity effect
		velocity.y -= gravity * Time.deltaTime;
		if (velocity.y <= -100f) {
			velocity.y = -100f;
		}

		// reset vertical velocity value when grounded
		if (isGrounded && velocity.y < 0) {
			velocity.y = 0f;
		}

		// move player according to gravity
		characterBody.Move(velocity * Time.deltaTime);
	}

	// handle mouse movement to rotate camera
	void cameraControl() {
		// rotate player about y and playercam about x
		//get axis values from input
		float mouseX = pauseUI.isPaused ? 0 : Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; //deltatime used for fps correction
		float mouseY = pauseUI.isPaused ? 0 : Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		// invert vertical rotation and restrict up/down
		xRot -= mouseY;
		xRot = Mathf.Clamp(xRot, -90f, 90f);
		
		// apply rotation
		cameraHolder.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
		transform.Rotate(Vector3.up * mouseX); //rotate player about y axis with mouseX movement
	}

	/*****************
	* Button Presses *
	******************/

	void UpdateCooldowns()
	{
		ab1Cooldown = (ab1Cooldown > 0) ? (ab1Cooldown - Time.deltaTime) : 0;
		ab2Cooldown = (ab2Cooldown > 0) ? (ab2Cooldown - Time.deltaTime) : 0;
	}

	void KeyControl()
	{
		// Keypress '1' -> time jump backward
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			// only allow time travel backwards if it doesn't go past the beginning.
			if (SceneManager.GetActiveScene().name == "GameScene" && ab2Cooldown <= 0 &&
				timeTravel.GetRealityTick() - (float) timeJumpAmount >= 0)
			{ 
				jumpBackwards();
			}
		}

		// Keypress '2' -> time jump forward
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			// only allow time travel forwards if it doesn't go past the end.
			if (SceneManager.GetActiveScene().name == "GameScene" && ab1Cooldown <= 0 &&
				timeTravel.GetRealityTick() + (float) timeJumpAmount <= timeTravel.GetCurrentTick())
			{
				jumpForward(); 
			}
		}

		// Left mouse click -> start grabbing
		if (Input.GetMouseButtonDown(0))
		{
			// if grabbing, check for intersection with player
			if (!damageWindow)
			{
				Collider[] playersGrab = Physics.OverlapSphere(grabCheck.position, grabCheckRadius, grabMask);
				foreach (var playerGotGrab in playersGrab)
				{
					// call grabplayer function on that player
					PlayerMovement targetPlayer = playerGotGrab.GetComponent<PlayerMovement>();
					if (team == (int) GameController.Teams.Seeker && 
						targetPlayer.team == (int) GameController.Teams.Hider)
					{
						targetPlayer.getFound();
					}
				}
				playerAnim.SetBool("isGrabbing", true);
			}
		}

		// Keypress 'e' -> start game
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient)
			{
				hud.StartCountingDown();
			}
		}

		// Keypress `ESC` -> stop counting down to game launch
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			hud.StopCountingDown();
		}

		// Keypress 'p' -> toggle debug mode
		if (Input.GetKeyDown(KeyCode.P))
		{
			hud.ToggleDebug();
		}
	}

	// change player teams
	public void changeTeam() {
		if (team == (int) GameController.Teams.Hider) {
			team = (int) GameController.Teams.Seeker;
			material.SetMaterial("seeker");
			hud.SetTeam("SEEKER");
		} else {
			team = (int) GameController.Teams.Hider;
			material.SetMaterial("hider");
			hud.SetTeam("HIDER");
		}
	}

	/************
	* RPC Calls *
	*************/

	[PunRPC]
	void RPC_jumpBackwards() {
		timeTravel.TimeJump(-timeJumpAmount);
		particles.StartJumpingBackward();
		ab2Cooldown = 15;
		hud.PressForwardJumpButton();
		game.otherPlayersElapsedTime[view.ViewID] -= timeJumpAmount / timeTravel.MaxTick();
	}

	public void jumpBackwards() {
		view.RPC("RPC_jumpBackwards", RpcTarget.All);
	}

	[PunRPC]
	void RPC_jumpForward() {
		timeTravel.TimeJump(timeJumpAmount);
		particles.StartJumpingForward();
		ab1Cooldown = 15;
		hud.PressBackJumpButton();
		game.otherPlayersElapsedTime[view.ViewID] += timeJumpAmount / timeTravel.MaxTick();
	}

	public void jumpForward() {
		view.RPC("RPC_jumpForward", RpcTarget.All);
	}

	// RPC function to be called when another player finds this one
	[PunRPC]
	void RPC_getFound() {
		changeTeam();
	}

	// RPC function to be called when another player hits this one
	// function to get found by calling RPC on all machines
	public void getFound(){
		view.RPC("RPC_getFound", RpcTarget.All);
	}

	// RPC function to be called by other machines to set this players transform
	[PunRPC]
	void RPC_movePlayer(Vector3 pos, Vector3 rot) {
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
		cameraHolder.transform.rotation = Quaternion.Euler(rot);
	}

	// function to move this player by calling RPC for all others
	public void movePlayer(Vector3 pos, Vector3 rot) {
		view.RPC("RPC_movePlayer", RpcTarget.All, pos, rot);
	}

	// function to enable player to damage others
	public void startGrabbing() {
		damageWindow = true;
	}

	// function to disable player to damage others
	public void stopGrabbing() {
		damageWindow = false;
		playerAnim.SetBool("isGrabbing", false);
	}

	// function called on game gameEnded
	public void onGameEnded(){
		/*if(PhotonNetwork.IsMasterClient){
			PhotonNetwork.LoadLevel("PreGameScene");
		}*/
	}
}
