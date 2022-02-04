using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ServerButton : MonoBehaviourPunCallbacks{

	public string serverName;
	public Text textField;
	public Button button;

	// Start is called before the first frame update
	void Start() {
		textField.text = serverName;
		button.onClick.AddListener(delegate { JoinRoom(); });
	}

	//user presses join room button
	public void JoinRoom(){
		PhotonNetwork.JoinRoom(serverName);
	}

	//when user connects to room - load scene as level
	public override void OnJoinedRoom(){
		PhotonNetwork.LoadLevel("GameScene");
	}
}
