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
	public Image healthbar;
	public Image enemyHealthbar;
	public Canvas enemyHealthbarContainer;
	// public Text enemyScoreDispl;
	// private int score = 0;
	private float speed = 5f;
	private float gravity = 10f;
	private float jumpPower = 10f;
	private float groundCheckRadius = 0.2f;
	private bool isGrounded = true;
	private Vector3 velocity;
	private float xRot = 0f;
	private Vector3 lastPos;
	private float health = 100f;




	//variables corresponding to the player's UI/HUD
	public Canvas UI;
	public PauseManager pauseUI;
	public Text debugMenu_speed;
	public Text debugMenu_room;
	public Text debugMenu_sprint;
	public Text debugMenu_hit;
	public Text debugMenu_ground;
	public Text debugMenu_health;
	public Text masterClientOpts;
	// public Text scoreDispl;
	private float secondsTillGame;
	private bool isCountingTillGameStart;


	//variables corresponding to player Animations
	public Animator playerAnim_hit;
	public bool damageWindow = false;
	public Transform hitCheck;
	public float hitCheckRadius = 1f;
	public LayerMask hitMask;

	//the photonView component that syncs with the network
	public PhotonView view;

	// Start is called before the first frame update
	void Start() {
		//define the photonView component
		view = GetComponent<PhotonView>();
		if(!view.IsMine){
			//destroy other player cameras and ui in local environment
			Destroy(cam.gameObject);
			Destroy(UI);
			gameObject.layer = 7;
		} else {
			//destroy playerhealthbar for enemies
			Destroy(enemyHealthbarContainer);
			gameObject.tag = "Client";
		}
		//allow master client to move players from one scene to another
		PhotonNetwork.AutomaticallySyncScene = true;
		//lock players cursor to center screen
		Cursor.lockState = CursorLockMode.Locked;
	}

	// Update is called once per frame
	void Update() {
		//local keys only affect client's player
		if(view.IsMine){
			//update lastPos from prev frame
			lastPos = transform.position;



			//move player



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

			//reset vertical velocity value when grounded
			if(isGrounded && velocity.y < 0){
				velocity.y = 0.2f;
			}
			//jump control
			if(Input.GetButtonDown("Jump") && isGrounded && !pauseUI.isPaused){
				velocity.y += Mathf.Sqrt(jumpPower * 2f * gravity);
			}
			//gravity effect
			velocity.y -= gravity * Time.deltaTime;
			//move player according to gravity
			characterBody.Move(velocity * Time.deltaTime);



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



			//handle other user inputs



			//start hit animation on click
			if(Input.GetMouseButtonDown(0)){
				playerAnim_hit.SetBool("isSpinning", true);
			}

			//start game onpress 'e'
			if(SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.E) && !isCountingTillGameStart){
				isCountingTillGameStart = true;
				secondsTillGame = 5.0f;
			}

			//if counting and user presses esc - stop
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


			//attack handler

			//if hitting, check for intersection with player
			if(damageWindow){
				Collider[] playersHit = Physics.OverlapSphere(hitCheck.position, hitCheckRadius, hitMask);
				foreach (var playerGotHit in playersHit){
					//call hitplayer function on that player
					playerGotHit.GetComponent<PlayerMovement>().hitPlayer(1.0f, view.ViewID);
				}
			}
		}
	}

	// LateUpdate is called once per frame after all rendering
	void LateUpdate() {

		if(view.IsMine){


			//update player HUD


			//if master client, show 'press e o start' text or 'starting in' text
			masterClientOpts.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient);
			// scoreDispl.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
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
			debugMenu_hit.text = "Hit: " + damageWindow;
			debugMenu_ground.text = "Ground: " + isGrounded;
			debugMenu_health.text = "Health: " + health;
			//update player score
			// scoreDispl.text = "" + score;

			//update health bar local and enemy, transform enemy tetures to billboard locally
			healthbar.rectTransform.sizeDelta = new Vector2(health*2, 30);
		} else {
			//update ui elements of enemies on clients machine
			enemyHealthbar.rectTransform.sizeDelta = new Vector2(health*10, 200);
			enemyHealthbarContainer.transform.LookAt(Camera.main.transform.position);
			// enemyScoreDispl.text = "" + score;
			//enemyHealthbarContainer.transform.rotation = Quaternion.Inverse(enemyHealthbarContainer.transform.rotation);
		}
	}

	// [PunRPC]
	// void RPC_incrScore(int scoreIncr){
	// 	score += scoreIncr;
	// }

	//RPC function to be called when another player hits this one
	[PunRPC]
	void RPC_getHit(float damage, int attackerID){
		//if(view.IsMine){
			health -= damage;
			if(health <= 0){
				health = 100f;
				transform.position = new Vector3(0, 10, 0);
				//increment score of player who damaged
				// PhotonView.Find(attackerID).RPC("RPC_incrScore", RpcTarget.All, 1);
			}
		//}
	}

	//function to take damage by calling RPC on all machines
	public void hitPlayer(float damage, int attackerID){
		view.RPC("RPC_getHit", RpcTarget.All, damage, attackerID);
	}

	//function to enable player to damage others
	public void startHitting(){
		damageWindow = true;
	}

	//function to disable player to damage others
	public void stopHitting(){
		damageWindow = false;
		playerAnim_hit.SetBool("isSpinning", false);
	}

}
