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
	public Canvas UI;
	public Transform groundCheck;
	public LayerMask groundMask;
	public float mouseSensitivity = 100f;
	public GameObject playerBody;
	public GameObject playerArm;
	public GameObject handThumb;
	public GameObject handThumbTip;
	public GameObject handIndex;
	public GameObject handIndexTip;
	public GameObject handMiddle;
	public GameObject handMiddleTip;

	public int team = (int) GameController.Teams.Seeker; // 0 seeker 1 hider // initialised to 0 but changeTeam is called on start to sync values
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
	private float ab3Cooldown = 0f;
	private int timeJumpAmount = 100;

	// references to materials for user team id
	public Material seekerMat;
	public Material hiderMat;

	// variables corresponding to UI
	public PauseManager pauseUI;

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

	// Start is called before the first frame update
	void Start() {
		DontDestroyOnLoad(this.gameObject);
		changeTeam(); // set the player's colour depending on their team
		view = GetComponent<PhotonView>(); // define the photonView component
		if (!view.IsMine) {
			// destroy other player cameras and ui in local environment
			Destroy(cam.gameObject);
			Destroy(cam.gameObject.GetComponent<AudioListener>());
			Destroy(UI);
			gameObject.layer = 7;
			playerBody.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		} else {
			gameObject.tag = "Client";
			playerBody.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		}
        PhotonNetwork.AutomaticallySyncScene = true;      // allow master client to move players from one scene to another
        Cursor.lockState = CursorLockMode.Locked;         // lock players cursor to center screen
        SceneManager.activeSceneChanged += onSceneChange; // link scenechange event to onscenechange

		hiderMat.SetFloat("_CutoffHeight", 50.0f);
		seekerMat.SetFloat("_CutoffHeight", 50.0f);
	}

	// onSceneChange is called by the SceneManager.activeSceneChanged event;
	void onSceneChange(Scene current, Scene next) {
		if (next.name == "GameScene") {
			game = FindObjectOfType<GameController>();
			timeTravel.connectToTimeLord();
			if (game == null) {
				Debug.Log("FUCK");
			}
			ab1Cooldown = 15;
			ab2Cooldown = 15;
			ab3Cooldown = 3;
		}
	}

	// Update is called once per frame
	void Update() {
		// local keys only affect client's player
		if (view.IsMine) {
			if (SceneManager.GetActiveScene().name == "PreGameScene" ||
			(SceneManager.GetActiveScene().name == "GameScene" && !game.gameEnded)) {
				movementControl();
				cameraControl();
				keyControl();
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

			// update player ability displays
			float[] abilityValues = new float[]{ab1Cooldown, ab2Cooldown, ab3Cooldown};
			hud.SetAbilityValues(abilityValues);

			// update pauseUI and cursor lock if game is ended
			if (SceneManager.GetActiveScene().name == "GameScene" && game.gameEnded)
			{
				pauseUI.isPaused = true;
				pauseUI.pauseMenuUI.SetActive(true);
				Cursor.lockState = CursorLockMode.None;
			}
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

		//invert vertical rotation and restrict up/down
		xRot -= mouseY;
		xRot = Mathf.Clamp(xRot, -90f, 90f);
		//apply rotation
		cam.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
		transform.Rotate(Vector3.up * mouseX); //rotate player about y axis with mouseX movement
	}

	/*****************
	* Button Presses *
	******************/

	void keyControl(){
		// only allow movement after game has started
		if(SceneManager.GetActiveScene().name == "PreGameScene" || 
		  (SceneManager.GetActiveScene().name == "GameScene" && game.gameStarted)) {
			// set cooldown values
			ab1Cooldown = (ab1Cooldown > 0) ? (ab1Cooldown - Time.deltaTime) : 0;
			ab2Cooldown = (ab2Cooldown > 0) ? (ab2Cooldown - Time.deltaTime) : 0;
			ab3Cooldown = (ab3Cooldown > 0) ? (ab3Cooldown - Time.deltaTime) : 0;

			// handle ability buttonpresses
			if(Input.GetKeyDown(KeyCode.Alpha1) && ab1Cooldown <= 0) {
				// only allow time travel forwards if it doesn't go past the end.
				if(SceneManager.GetActiveScene().name == "GameScene" && 
				   timeTravel.GetRealityTick() + (float) timeJumpAmount <= timeTravel.GetCurrentTick()) {
					jumpForward(); 
				}
			}

			if (Input.GetKeyDown(KeyCode.Alpha2) && ab2Cooldown <= 0) {
				// only allow time travel backwards if it doesn't go past the beginning.
				if(SceneManager.GetActiveScene().name == "GameScene" && 
				   timeTravel.GetRealityTick() - (float) timeJumpAmount >= 0) { 
					jumpBackwards();
				}
			}

			if (Input.GetKeyDown(KeyCode.Alpha3) && ab3Cooldown <= 0) {
				ab3Cooldown = 3;
			}
			// start grab animation on click
			if (Input.GetMouseButtonDown(0)) {
				// if grabbing, check for intersection with player
				if (!damageWindow) {
					Collider[] playersGrab = Physics.OverlapSphere(grabCheck.position, grabCheckRadius, grabMask);
					foreach (var playerGotGrab in playersGrab) {
						// call grabplayer function on that player
						PlayerMovement targetPlayer = playerGotGrab.GetComponent<PlayerMovement>();
						if (team == (int) GameController.Teams.Seeker && 
						    targetPlayer.team == (int) GameController.Teams.Hider) {
							targetPlayer.getFound();
						}
					}
					playerAnim.SetBool("isGrabbing", true);
				}
			}

			// Start game onpress 'e'
			if (SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient &&
				Input.GetKeyDown(KeyCode.E)) {
				hud.StartCountingDown();
			}

			// If counting for game launch and user presses esc - stop
			if (Input.GetKeyDown(KeyCode.Escape)) {
				hud.StopCountingDown();
			}
		}
	}

	// change player teams
	public void changeTeam() {
		if (team == (int) GameController.Teams.Hider) {
			team = (int) GameController.Teams.Seeker;
			playerBody.GetComponent<Renderer>().material = seekerMat;
			playerArm.GetComponent<Renderer>().material = seekerMat;
			
			handIndex.GetComponent<Renderer>().material = seekerMat;
			handIndexTip.GetComponent<Renderer>().material = seekerMat;
			handMiddle.GetComponent<Renderer>().material = seekerMat;
			handMiddleTip.GetComponent<Renderer>().material = seekerMat;
			handThumb.GetComponent<Renderer>().material = seekerMat;
			handThumbTip.GetComponent<Renderer>().material = seekerMat;

			hud.SetTeam("SEEKER");
		} else {
			team = (int) GameController.Teams.Hider;
			playerBody.GetComponent<Renderer>().material = hiderMat;
			playerArm.GetComponent<Renderer>().material = hiderMat;
			
			handIndex.GetComponent<Renderer>().material = hiderMat;
			handIndexTip.GetComponent<Renderer>().material = hiderMat;
			handMiddle.GetComponent<Renderer>().material = hiderMat;
			handMiddleTip.GetComponent<Renderer>().material = hiderMat;
			handThumb.GetComponent<Renderer>().material = hiderMat;
			handThumbTip.GetComponent<Renderer>().material = hiderMat;

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
		cam.transform.rotation = Quaternion.Euler(rot);
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
