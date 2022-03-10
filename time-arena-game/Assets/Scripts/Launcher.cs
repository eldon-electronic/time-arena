using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

/**
* Launcher connects to Photon services. Allows us to find other games and players on the server.
*/

// PunCallbacks gives us access to callbacks for joining lobbys, room creation, errors etc.
public class Launcher : MonoBehaviourPunCallbacks {

	public static Launcher Instance;

	void Awake() {
		Instance = this;
	}

	[SerializeField] private TMP_InputField _roomNameInput;
	[SerializeField] private TMP_Text _errorText;
	[SerializeField] private TMP_Text _roomNameText;
	[SerializeField] private Transform _roomListContainer;
	[SerializeField] private GameObject _roomListItemPrefab;
	[SerializeField] private Transform _playerListContainer;
	[SerializeField] private GameObject _playerListItemPrefab;
	[SerializeField] private TMP_Text _creationFailedText;
	[SerializeField] private Button _startButton;

	// Start is called before the first frame update
	void Start() {
		Debug.Log("Connecting to Master");
		PhotonNetwork.ConnectUsingSettings();
	}

	public void StartGame() {
		// Level index of PreGameScene is 1, check build settings
		PhotonNetwork.LoadLevel(1);
	}

	public override void OnConnectedToMaster() {
		Debug.Log("Connected to Master");
		PhotonNetwork.JoinLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public override void OnJoinedLobby() {
		Debug.Log("Joined Lobby");
		MenuManager.Instance.OpenMenu("mainMenu");
		PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
	}

	public void CreateRoom() {
		if (string.IsNullOrEmpty(_roomNameInput.text)) {
			return;
		}

		PhotonNetwork.CreateRoom(_roomNameInput.text);
		MenuManager.Instance.OpenMenu("loadingMenu");
	}

    public override void OnJoinedRoom() {
		MenuManager.Instance.OpenMenu("roomMenu");
		_roomNameText.text = PhotonNetwork.CurrentRoom.Name;

		Player[] players = PhotonNetwork.PlayerList;
		
		// Purge old player list
		foreach (Transform playerListItem in _playerListContainer) {
			Destroy(playerListItem.gameObject);
		}

		// Refresh with new one
		for (int i = 0; i < players.Length; i++) {
			Instantiate(_playerListItemPrefab, _playerListContainer).GetComponent<PlayerListItem>().SetUp(players[i]);
		}

		_startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

	public override void OnMasterClientSwitched(Player newMasterPlayer) {
		_startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
	}

	// Helper function to disable text pop-up OnCreateRoomFailed
	private void disableErrorText() {
		_creationFailedText.gameObject.SetActive(false);
	}

	public override void OnCreateRoomFailed(short returnCode, string returnMessage) {
		MenuManager.Instance.OpenMenu("createRoomMenu");
		string debugText = "Error code [" + returnCode.ToString() + "] Could not create room: " + returnMessage;
		if (returnCode == (short) 32766) { // Error code when creating a room that already exists
			_creationFailedText.text = "ROOM CREATION FAILED: " + _roomNameInput.text + " already exists...";
			_creationFailedText.gameObject.SetActive(true);
			Invoke("disableErrorText", 3f);
		} else {
			_errorText.text = debugText;
			MenuManager.Instance.OpenMenu("errorMenu");
		}
	}

	public void JoinRoom(RoomInfo info) {
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("loadingMenu");
	}

	public void LeaveRoom() {
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("loadingMenu");
	}

    public override void OnLeftRoom() {
		MenuManager.Instance.OpenMenu("mainMenu");
    }

	public override void OnRoomListUpdate(List<RoomInfo> rooms) {
		// Destroy all the items in the room list
		foreach (Transform transform in _roomListContainer) {
			Destroy(transform.gameObject);
		}

		// Refresh the room container
		for (int i = 0; i < rooms.Count; i++) {
			// Photon doesn't remove rooms, they store the state of their existence in a list of bools
			if (rooms[i].RemovedFromList) continue;
			Instantiate(_roomListItemPrefab, _roomListContainer).GetComponent<RoomListItem>().SetUp(rooms[i]);
		}
	}
}
