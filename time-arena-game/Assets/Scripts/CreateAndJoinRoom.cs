using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks{

	//values for input text fields set by user
	public InputField nameInput;
	public InputField roomInput;

	// username
	public System.String username;

	//user presses create room button
	public void CreateRoom(){
		if (roomInput.text != "" && nameInput.text != "") {
			username = nameInput.text;
			PhotonNetwork.CreateRoom(roomInput.text);
		}
	}

	//user presses join room button
	public void JoinRoom(){
		if (roomInput.text != "" && nameInput.text != "") {
			username = nameInput.text;
			PhotonNetwork.JoinRoom(roomInput.text);
		}
	}

	//when user connects to room - load scene as level
	public override void OnJoinedRoom(){
		PhotonNetwork.LoadLevel("PreGameScene");
	}

	//onpress of back button - return to home screen and disconnect
	public void Back(){
		SceneManager.LoadScene("MenuScene");
		PhotonNetwork.Disconnect();
	}

}
