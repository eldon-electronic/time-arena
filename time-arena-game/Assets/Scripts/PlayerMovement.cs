using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	Vector3 velocity;

	//the photonView component that syncs with the network
	public PhotonView view;


	// Start is called before the first frame update
	void Start() {
		//define the photonView component
		view = GetComponent<PhotonView>();
		//destroy other player cameras in local environment
		if(!view.IsMine){
			Destroy(cam);
		}
	}

	// Update is called once per frame
	void Update() {
		//local keys only affect client's player
		if(view.IsMine){
			//sprint speed
			if(Input.GetKey("left shift")){
				speed = 10f;
			} else {
				speed = 5f;
			}

			//get movement axis values
			float xMove = Input.GetAxis("Horizontal");
			float zMove = Input.GetAxis("Vertical");
			//check if player's GroundCheck intersects with any environment object
			isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

			//set and normalise movement vector
			Vector3 movement = (transform.right * xMove) + (transform.forward * zMove);
			if(movement.magnitude != 1){
				movement /= movement.magnitude;
			}
			//transform according to movement vector
			characterBody.Move(movement * speed * Time.deltaTime);

			//reset vertical velocity value when grounded
			if(isGrounded && velocity.y < 0){
				velocity.y = 0.2f;
			}
			//jump control
			if(Input.GetButtonDown("Jump") && isGrounded){
				velocity.y += Mathf.Sqrt(jumpPower * 2f * gravity);
			}
			//gravity effect
			velocity.y -= gravity * Time.deltaTime;
			//move player according to gravity
			characterBody.Move(velocity * Time.deltaTime);
		}
	}
}
