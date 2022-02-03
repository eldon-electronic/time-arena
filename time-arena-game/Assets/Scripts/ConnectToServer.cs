using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ConnectToServer : MonoBehaviourPunCallbacks {

	// Start is called before the first frame update
	void Start(){
		//connect to photon server - settings defined in unity
		PhotonNetwork.ConnectUsingSettings();
	}

	//when connected to server join the lobby
	public override void OnConnectedToMaster(){
		PhotonNetwork.JoinLobby();
	}

	//when in lobby move to lobby scene
	public override void OnJoinedLobby(){
		SceneManager.LoadScene("LobbyScene");
	}
}
