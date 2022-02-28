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


	private Color transRed = new Color(1.0f, 0.0f, 0.0f, 0.5f);

	//user presses join or create room button
	public void JoinOrCreateRoom() {
		if (roomInput.text != "" && nameInput.text != "") {
			PhotonNetwork.NickName = nameInput.text;
			PhotonNetwork.JoinOrCreateRoom(roomInput.text, null, null);
		} else {
			if (nameInput.text == "") nameInput.placeholder.color = transRed;
			if (roomInput.text == "") roomInput.placeholder.color = transRed;
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
