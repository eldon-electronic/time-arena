using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks{

	//values for input text fields set by user
	public InputField createInput;
	public InputField joinInput;

	//user presses create room button
	public void CreateRoom(){
		PhotonNetwork.CreateRoom(createInput.text);
	}

	//user presses join room button
	public void JoinRoom(){
		PhotonNetwork.JoinRoom(joinInput.text);
	}

	//when user connects to room - load scene as level
	public override void OnJoinedRoom(){
		PhotonNetwork.LoadLevel("GameScene");
	}

}
