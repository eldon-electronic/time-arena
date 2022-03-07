using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
 
/**
* Launcher connects to Photon services. Allows us to find other games and players on the server.
*/

// PunCallbacks gives us access to callbacks for joining lobbys, room creation, errors etc.
public class Launcher : MonoBehaviourPunCallbacks {

	// Start is called before the first frame update
	void Start() {
		Debug.Log("Connecting to Master");
		PhotonNetwork.ConnectUsingSettings();
	}

	// Called when actually connected to master server
	public override void OnConnectedToMaster(){
		Debug.Log("Connected to Master");
		PhotonNetwork.JoinLobby();
	}

	// When in lobby move to lobby scene
	public override void OnJoinedLobby() {
		Debug.Log("Joined Lobby");
		SceneManager.LoadScene("LobbyScene");

	}
}
