using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour {

	// variables defining player values
	public CharacterController characterBody;
	public Camera cam;
	public Transform groundCheck;
	public LayerMask groundMask;
	public float mouseSensitivity = 100f;
	public GameObject playerBody;
	public int team = 0;//0 seeker 1 hider //iniitialised to 0 but changeTeam is called on start to sync values
	private float speed = 5f;
	private float gravity = 10f;
	private float jumpPower = 10f;
	private float groundCheckRadius = 0.5f;
	private bool isGrounded = true;
	private Vector3 velocity;
	private float xRot = 0f;
	private Vector3 lastPos;
	private float ab1Cooldown = 0f;
	private float ab2Cooldown = 0f;
	private float ab3Cooldown = 0f;

	//references to materials for user team id
	public Material seekerMat;
	public Material hiderMat;

	//variables corresponding to the player's UI/HUD
	public Canvas UI;
	public PauseManager pauseUI;
	public Text debugMenu_speed;
	public Text debugMenu_room;
	public Text debugMenu_sprint;
	public Text debugMenu_grab;
	public Text debugMenu_ground;
	public Text masterClientOpts;
	public Text ab1Cooldown_displ;
	public Text ab2Cooldown_displ;
	public Text ab3Cooldown_displ;
	public Text teamDispl;
	public Text timeDispl;
	public Text startTimeDispl;
	public Text winningDispl;
	private float secondsTillGame;
	private bool isCountingTillGameStart;


	//variables corresponding to player Animations
	public Animator playerAnim_grab;
	public Transform grabCheck;
	public LayerMask grabMask;
	private float grabCheckRadius = 1f;
	private bool damageWindow = false;

	//the photonView component that syncs with the network
	public PhotonView view;

	//variables corresponding to the gamestate
	public GameController game;

	// Start is called before the first frame update
	void Start() {
		DontDestroyOnLoad(this.gameObject);
		//set the player's colour depending on their team
		changeTeam();
		//define the photonView component
		view = GetComponent<PhotonView>();
		if(!view.IsMine){
			//destroy other player cameras and ui in local environment
			Destroy(cam);
			Destroy(cam.gameObject.GetComponent<AudioListener>());
			Destroy(UI);
			gameObject.layer = 7;
		} else {
			gameObject.tag = "Client";
		}
		//allow master client to move players from one scene to another
		PhotonNetwork.AutomaticallySyncScene = true;
		//lock players cursor to center screen
		Cursor.lockState = CursorLockMode.Locked;
		//link scenechange event to onscenechange
		SceneManager.activeSceneChanged += onSceneChange;
	}

	// onSceneChange is called by the SceneManager.activeSceneChanged event;
	void onSceneChange(Scene current, Scene next){
		if(next.name == "GameScene"){
			game = FindObjectOfType<GameController>();
			if(game == null){
				Debug.Log("FUCK");
			}
		}
	}

	// Update is called once per frame
	void Update() {
		//local keys only affect client's player
		if(view.IsMine){
			if(SceneManager.GetActiveScene().name == "PreGameScene" || (SceneManager.GetActiveScene().name == "GameScene" && !game.gameEnded)){
				movementControl();
				cameraControl();
				keyControl();
			}
		}
	}

	// LateUpdate is called once per frame after all rendering (for UI mainly)
	void LateUpdate() {

		if(view.IsMine){


			//update player HUD


			//if master client, show 'press e o start' text or 'starting in' text
			masterClientOpts.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient);
			teamDispl.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
			timeDispl.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
			startTimeDispl.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
			if(isCountingTillGameStart){
				masterClientOpts.text = "Starting in " + System.Math.Round (secondsTillGame, 0) + "s";
				if(System.Math.Round (secondsTillGame, 0) <= 0.0f){
					//PhotonNetwork.Room.open = false;
					masterClientOpts.text = "Loading...";
				}
			}
			//update debug menu settings
			Vector3 movementVector = transform.position - lastPos;
			float distTravelled = movementVector.magnitude / Time.deltaTime;
			debugMenu_speed.text = "Speed: " + distTravelled;
			debugMenu_room.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
			debugMenu_sprint.text = "Sprint: " + Input.GetKey("left shift");
			debugMenu_grab.text = "Grab: " + damageWindow;
			debugMenu_ground.text = "Ground: " + isGrounded;

			//update player ability displays
			ab1Cooldown_displ.text = "" + (int)ab1Cooldown;
			ab2Cooldown_displ.text = "" + (int)ab2Cooldown;
			ab3Cooldown_displ.text = "" + (int)ab3Cooldown;

			//update winningTeam Text

			//update gametimer
			if(SceneManager.GetActiveScene().name == "GameScene"){
				float t = game.gameLength - game.timeElapsedInGame;
				startTimeDispl.transform.parent.gameObject.SetActive(!game.gameStarted);
				if(game.gameStarted && !game.gameEnded){
					timeDispl.text = (int)(t/60) + ":" + ((int)(t%60)).ToString().PadLeft(2, '0') + ":" + (((int)(((t%60)-(int)(t%60))*100))*60/100).ToString().PadLeft(2, '0');
				} else if(game.gameEnded){
					winningDispl.transform.parent.gameObject.SetActive(true);
					winningDispl.text = (game.winningTeam == 1) ? "HIDERS WIN!" : "SEEKERS WIN!";
					pauseUI.isPaused = true;
					pauseUI.pauseMenuUI.SetActive(true);
					Cursor.lockState = CursorLockMode.None;
				} else {
					startTimeDispl.text = "" + (5-(int)(game.timeElapsedInGame+0.9f));
					timeDispl.text = "0:00:00";
				}
			}
		}
	}

	//handle movement axis inputs (wasd, arrowkeys, joystick)
	void movementControl(){
		//update lastPos from prev frame
		lastPos = transform.position;
		//only allow movement after game has started
		if(SceneManager.GetActiveScene().name == "PreGameScene" || (SceneManager.GetActiveScene().name == "GameScene" && game.gameStarted)){
			//sprint speed
			if(Input.GetKey("left shift")){
				speed = 10f;
			} else {
				speed = 5f;
			}
			//get movement axis values
			float xMove = pauseUI.isPaused ? 0 : Input.GetAxis("Horizontal");
			float zMove = pauseUI.isPaused ? 0 : Input.GetAxis("Vertical");
			//check if player's GroundCheck intersects with any environment object
			isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

			//set and normalise movement vector
			Vector3 movement = (transform.right * xMove) + (transform.forward * zMove);
			if(movement.magnitude != 1 && movement.magnitude != 0){
				movement /= movement.magnitude;
			}
			//transform according to movement vector
			characterBody.Move(movement * speed * Time.deltaTime);
		}
		//jump control
		if(Input.GetButtonDown("Jump") && isGrounded && !pauseUI.isPaused){
			velocity.y += Mathf.Sqrt(jumpPower * 2f * gravity);
		}
		//gravity effect
		velocity.y -= gravity * Time.deltaTime;
		if(velocity.y <= -100f){
			velocity.y = -100f;
		}
		//reset vertical velocity value when grounded
		if(isGrounded && velocity.y < 0){
			velocity.y = 0f;
		}
		//move player according to gravity
		characterBody.Move(velocity * Time.deltaTime);
	}

	//handle mouse movement to rotate camera
	void cameraControl(){
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

	//handle all other button presses for abilities and UI
	void keyControl(){
		//only allow movement after game has started
		if(SceneManager.GetActiveScene().name == "PreGameScene" || (SceneManager.GetActiveScene().name == "GameScene" && game.gameStarted)){
			//set cooldown values
			ab1Cooldown=(ab1Cooldown > 0) ? (ab1Cooldown - Time.deltaTime) : 0;
			ab2Cooldown=(ab2Cooldown > 0) ? (ab2Cooldown - Time.deltaTime) : 0;
			ab3Cooldown=(ab3Cooldown > 0) ? (ab3Cooldown - Time.deltaTime) : 0;
			//handle ability buttonpresses
			if(Input.GetKeyDown(KeyCode.Alpha1) && ab1Cooldown <= 0){
				//Debug.Log(characterBody.gameObject.ToString());
				//TomBaker.timeJump(characterBody.gameObject, 30);
				changeTeam();
				ab1Cooldown = 9;
			}
			if(Input.GetKeyDown(KeyCode.Alpha2) && ab2Cooldown <= 0){
				//Debug.Log(characterBody.gameObject.ToString());
				//TomBaker.timeJump(characterBody.gameObject, 30);
				ab2Cooldown = 5;
			}
			if(Input.GetKeyDown(KeyCode.Alpha3) && ab2Cooldown <= 0){
				//Debug.Log(characterBody.gameObject.ToString());
				//TomBaker.timeJump(characterBody.gameObject, 30);
				ab3Cooldown = 3;
			}
			//start grab animation on click
			if(Input.GetMouseButtonDown(0)){
				//if grabbing, check for intersection with player
				if(!damageWindow){
					Collider[] playersGrab = Physics.OverlapSphere(grabCheck.position, grabCheckRadius, grabMask);
					foreach (var playerGotGrab in playersGrab){
						//call grabplayer function on that player
						PlayerMovement targetPlayer = playerGotGrab.GetComponent<PlayerMovement>();
						if(team == 0 && targetPlayer.team == 1){
							targetPlayer.getFound();
						}
					}
					playerAnim_grab.SetBool("isGrabbing", true);
				}
			}
			//start game onpress 'e'
			if(SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.E) && !isCountingTillGameStart){
				isCountingTillGameStart = true;
				secondsTillGame = 5.0f;
			}
			//if counting for game launch and user presses esc - stop
			if(Input.GetKeyDown(KeyCode.Escape)){
				isCountingTillGameStart = false;
				secondsTillGame = 0;
			}
			//if counting, reduce timer
			if(PhotonNetwork.IsMasterClient && isCountingTillGameStart){
				secondsTillGame -= Time.deltaTime;
				if(secondsTillGame <= 0){
					PhotonNetwork.LoadLevel("GameScene");
					isCountingTillGameStart = false;
				}
			}
		}
	}

	//change player teams
	public void changeTeam(){
		//if team is odd, set to 0, else set to 1
		if(team == 0){
			team = 1;
			playerBody.GetComponent<Renderer>().material = seekerMat;
			teamDispl.text = "SEEKER";
		} else {
			team = 0;
			playerBody.GetComponent<Renderer>().material = hiderMat;
			teamDispl.text = "HIDER";
		}
	}

	//RPC function to be called when another player finds this one
	[PunRPC]
	void RPC_getFound(){
		changeTeam();
	}

	//function to get found by calling RPC on all machines
	public void getFound(){
		view.RPC("RPC_getFound", RpcTarget.All);
	}

	//RPC function to be called by other machines to set this players transform
	[PunRPC]
	void RPC_movePlayer(Vector3 pos, Vector3 rot){
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
		cam.transform.rotation = Quaternion.Euler(rot);
	}

	//function to move this player by calling RPC for all others
	public void movePlayer(Vector3 pos, Vector3 rot){
		view.RPC("RPC_movePlayer", RpcTarget.All, pos, rot);
	}

	//function to enable player to damage others
	public void startGrabbing(){
		damageWindow = true;
	}

	//function to disable player to damage others
	public void stopGrabbing(){
		damageWindow = false;
		playerAnim_grab.SetBool("isGrabbing", false);
	}

	//function called on game gameEnded
	public void onGameEnded(){
		/*if(PhotonNetwork.IsMasterClient){
			PhotonNetwork.LoadLevel("PreGameScene");
		}*/
	}

}
