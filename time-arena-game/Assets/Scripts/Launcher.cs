using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
 using TMPro;

/**
* Launcher connects to Photon services. Allows us to find other games and players on the server.
*/

// PunCallbacks gives us access to callbacks for joining lobbys, room creation, errors etc.
public class Launcher : MonoBehaviourPunCallbacks {

	[SerializeField] TMP_InputField roomNameInput;
	[SerializeField] TMP_Text errorText;

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

		// Because we have an OpenMenu function that takes a string and not a GameObject
		// we can now open a menu simply by referencing its menuName string. This line
		// will open the main menu where users can find rooms, create a room or exit the game.
		// Because we have a public Instance of MenuManager, we can access functions simply
		// by accessing the Instance, and not an instantiated object:
		MenuManager.Instance.OpenMenu("mainMenu");
	}

	public void CreateRoom() {
		if (string.IsNullOrEmpty(roomNameInput.text)) return;
		PhotonNetwork.CreateRoom(roomNameInput.text);
		MenuManager.Instance.OpenMenu("loadingMenu");
	}

	// When we create a room there are two callbacks that can occur:

	// Callback when successfully created a room
    public override void OnJoinedRoom()
    {

    }

	// Callback when room creation failed
	public override void OnCreateRoomFailed(short returnCode, string returnMessage) {
		errorText.text = "Could not create room: " + returnMessage;
	}
}
