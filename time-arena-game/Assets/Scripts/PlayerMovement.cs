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
	public float speed = 5f;
	public float gravity = 10f;
	public float jumpPower = 10f;
	public Transform groundCheck;
	public float groundCheckRadius = 0.2f;
	public LayerMask groundMask;
	public bool isGrounded = true;
	public Vector3 velocity;
	public float mouseSensitivity = 100f;
	public float xRot = 0f;
	public Vector3 lastPos;
	public float health = 100f;
	public Image healthbar;

	//variables corresponding to the player's UI/HUD
	public PauseManager pauseUI;
	public Text debugMenu_speed;
	public Text debugMenu_room;
	public Text debugMenu_sprint;
	public Text debugMenu_hit;
	public Text debugMenu_ground;

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
		//destroy other player cameras in local environment
		if(!view.IsMine){
			Destroy(cam);
			gameObject.layer = 7;
		}
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



			//attack handler



			//start hit animation on click
			if(Input.GetMouseButtonDown(0)){
				playerAnim_hit.SetBool("isSpinning", true);
			}
			//if hitting, check for intersection with player
			if(damageWindow){
				Collider[] playersHit = Physics.OverlapSphere(hitCheck.position, hitCheckRadius, hitMask);
				foreach (var hitCollider in playersHit){
					hitCollider.GetComponent<PlayerMovement>().getHit();
				}
			}



			//update player HUD



			Vector3 movementVector = transform.position - lastPos;
			float distTravelled = movementVector.magnitude / Time.deltaTime;
			debugMenu_speed.text = "Speed: " + distTravelled;
			debugMenu_room.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
			debugMenu_sprint.text = "Sprint: " + Input.GetKey("left shift");
			debugMenu_hit.text = "Hit: " + damageWindow;
			debugMenu_ground.text = "Ground: " + isGrounded;

			healthbar.rectTransform.sizeDelta = new Vector2(health*2, 30);


		}
	}

	//function to take damage
	public void getHit(){
		health -= 10f;
		if(health <= 0){
			PhotonNetwork.LeaveRoom();
			SceneManager.LoadScene("LobbyScene");
		}
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
