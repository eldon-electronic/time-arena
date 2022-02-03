using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCamera : MonoBehaviour {
	//player camera values
	public float mouseSensitivity = 100f;
	public Transform characterBody;
	public float xRot = 0f;

	//photonView component of player (parent to camera)
	public PhotonView view;
	// Start is called before the first frame update
	void Start() {
		//lock cursor (esc negates)
		Cursor.lockState = CursorLockMode.Locked;
		//connect photonView with player photonView
		view = transform.parent.gameObject.GetComponent<PhotonView>();
	}

	// Update is called once per frame
	void Update() {
		//only move camera for client
		if(view.IsMine){
			//get axis values from input
			float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; //deltatime used for fps correction
			float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

			//invert vertical rotation and restrict up/down
			xRot -= mouseY;
			xRot = Mathf.Clamp(xRot, -90f, 90f);
			//apply rotation
			transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
			characterBody.Rotate(Vector3.up * mouseX); //rotate player about y axis with mouseX movement
		}
	}
}
