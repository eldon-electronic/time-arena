using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

/**
* Launcher connects to Photon services. Allows us to find other games and players on the server.
*/

// PunCallbacks gives us access to callbacks for joining lobbys, room creation, errors etc.
public class Launcher : MonoBehaviourPunCallbacks {

	public static Launcher Instance;
	[SerializeField] private TMP_InputField _roomNameInput;
	[SerializeField] private TMP_Text _errorText;
	[SerializeField] private TMP_Text _roomNameText;
	[SerializeField] private Transform _roomListContainer;
	[SerializeField] private GameObject _roomListItemPrefab;
	[SerializeField] private Transform _playerListContainer;
	[SerializeField] private GameObject _playerListItemPrefab;
	[SerializeField] private TMP_Text _creationFailedText;
	[SerializeField] private Button _startButton;
	[SerializeField] private Image _startButtonMask;
	[SerializeField] private TMP_Text _usernameErrorText;
	[SerializeField] private Sprite[] _teamIcons;
	private PhotonView _view;
	private Dictionary<string, string> _currentPlayerIcons = new Dictionary<string, string>();

	void Awake()
	{
		Instance = this;
		_view = PhotonView.Get(this);
	}

	void Start() 
	{
		if (!PhotonNetwork.IsConnected) {	
			Debug.Log("Connecting to Master");
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	void Update() 
	{
		_startButton.interactable = AllPlayersHaveTeams(_playerListContainer);
		_startButton.gameObject.GetComponent<StartButtonController>().isActive = AllPlayersHaveTeams(_playerListContainer);
	}

	// --------------- Callbacks ---------------

	public override void OnConnectedToMaster() {
		Debug.Log("Connected to Master");
		PhotonNetwork.JoinLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public override void OnJoinedLobby() {
		Debug.Log("Joined Lobby");
		MenuManager.Instance.OpenMenu("mainMenu");
	}

	public override void OnJoinedRoom() {
		MenuManager.Instance.OpenMenu("roomMenu");
		_roomNameText.text = PhotonNetwork.CurrentRoom.Name;

		UpdatePlayerList();

		_startButton.interactable = AllPlayersHaveTeams(_playerListContainer);
		_startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

	public override void OnMasterClientSwitched(Player newMasterPlayer) {
		_startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
		UpdateMaster(PhotonNetwork.IsMasterClient);
	}

	public override void OnPlayerEnteredRoom(Player newPlayer) { 
		Instantiate(_playerListItemPrefab, _playerListContainer)
			.GetComponent<PlayerListItem>()
			.SetUp(newPlayer, PhotonNetwork.IsMasterClient, _teamIcons);
	}

	public override void OnPlayerLeftRoom(Player oldPlayer) {
		foreach (Transform playerListTransform in _playerListContainer) {
			if (playerListTransform.gameObject.GetComponent<PlayerListItem>().username == oldPlayer.NickName) {
				Destroy(playerListTransform.gameObject);
			}
		}
	}

	public override void OnCreateRoomFailed(short returnCode, string returnMessage) {
		MenuManager.Instance.OpenMenu("createRoomMenu");
		string debugText = "Error code [" + returnCode.ToString() + "] Could not create room: " + returnMessage;
		if (returnCode == (short) 32766) { // Error code when creating a room that already exists
			_creationFailedText.text = "ROOM CREATION FAILED: " + _roomNameInput.text + " already exists...";
			_creationFailedText.gameObject.SetActive(true);
			Invoke("disableCreationErrorText", 3f);
		} else {
			_errorText.text = debugText;
			MenuManager.Instance.OpenMenu("errorMenu");
		}
	}

	public override void OnLeftRoom() {
		if (PhotonNetwork.IsConnected) {
			PhotonNetwork.Disconnect();
		} else {
			MenuManager.Instance.OpenMenu("mainMenu");
		}
	}

	public override void OnDisconnected(DisconnectCause cause) {
		Start();
	}

	public override void OnRoomListUpdate(List<RoomInfo> rooms) {
		UpdateRoomList(rooms);
	}

	// --------------- Public functions ---------------

	public void StartGame() {
		_view.RPC("RPC_saveIcons", RpcTarget.All);
		
		// Level index of PreGameScene is 1, check build settings
		PhotonNetwork.LoadLevel(1);
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void CreateRoom() {
		if (string.IsNullOrEmpty(_roomNameInput.text)) {
			return;
		}

		RoomOptions roomOptions = new RoomOptions();
		roomOptions.PublishUserId = true;
		roomOptions.MaxPlayers = 10;

		PhotonNetwork.CreateRoom(_roomNameInput.text, roomOptions);
		MenuManager.Instance.OpenMenu("loadingMenu");
	}

	// Not using MenuManager for opening the find room or create room menus, to check
	// if a user has set a nickname yet.
	public void OpenCreateRoomMenu() {
		if (string.IsNullOrEmpty(PhotonNetwork.NickName)) {
			_usernameErrorText.gameObject.SetActive(true);
			Invoke("disableUsernameErrorText", 3f);
		} else {
			MenuManager.Instance.OpenMenu("createRoomMenu");
		}
	}

	public void OpenFindRoomMenu() {
		if (string.IsNullOrEmpty(PhotonNetwork.NickName)) {
			_usernameErrorText.gameObject.SetActive(true);
			Invoke("disableUsernameErrorText", 3f);
		} else {
			MenuManager.Instance.OpenMenu("findRoomMenu");
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

	public void ReturnToMain() {
		MenuManager.Instance.OpenMenu("mainMenu");
	}

	public void UpdateIcons(string userID, string spriteName) {
		_view.RPC("RPC_updateIcon", RpcTarget.All, userID, spriteName);
	}

	// --------------- RPC and Events ---------------

	[PunRPC] void RPC_updateIcon(string userID, string spriteName) {
		Sprite newIcon = null;
		foreach (Sprite teamIcon in _teamIcons) {
			if (teamIcon.name == spriteName) newIcon = teamIcon;
		}

		foreach (Transform playerListTransform in _playerListContainer) {
			PlayerListItem playerListItem = playerListTransform.gameObject.GetComponent<PlayerListItem>();
			if (playerListItem.GetUserID() == userID)
			{
				playerListItem.SetTeamImage(newIcon);
			}
		}
	}

	[PunRPC] void RPC_getIcons() {
		_currentPlayerIcons.Clear();

		foreach (Transform playerListTransform in _playerListContainer) {
			PlayerListItem playerListItem = playerListTransform.gameObject.GetComponent<PlayerListItem>();
			_currentPlayerIcons.Add(playerListItem.GetUserID(), playerListItem.GetTeamImage().sprite.name);
			// Debug.Log($"Added {playerListItem.username}: {playerListItem.GetUserID()}, {playerListItem.teamImage.sprite.name} to dict");
		}

		// If you're the first to enter the room there are no PlayerListItems in the container,
		// then simply add yourself and specify no icon.
		if (_currentPlayerIcons.Count == 0) {
			Player[] players = PhotonNetwork.PlayerList;
			_currentPlayerIcons.Add(players[0].UserId, "no_team_icon");
		}

		_view.RPC("RPC_instantiatePlayers", RpcTarget.All, _currentPlayerIcons);
	}

	[PunRPC] void RPC_instantiatePlayers(Dictionary<string, string> playerIcons) {
		Player[] players = PhotonNetwork.PlayerList;
		foreach (Transform playerListItem in _playerListContainer) {
			Destroy(playerListItem.gameObject);
		}

		for (int i = 0; i < players.Length; i++) {
			Instantiate(_playerListItemPrefab, _playerListContainer)
				.GetComponent<PlayerListItem>()
				.SetUp(players[i], PhotonNetwork.IsMasterClient, _teamIcons, playerIcons[players[i].UserId]);
		}
	}

	[PunRPC] void RPC_saveIcons() {
		foreach (Transform playerListTransform in _playerListContainer) {
			PlayerListItem playerListItem = playerListTransform.gameObject.GetComponent<PlayerListItem>();
			PlayerPrefs.SetString(playerListItem.GetUserID(), playerListItem.GetTeamImage().sprite.name);
		}
	}

	// --------------- Private functions ---------------

	private void UpdatePlayerList() {
		foreach (Transform playerListItem in _playerListContainer) {
			Destroy(playerListItem.gameObject);
		}

		// Get icons from master and send out updates to all players
		_view.RPC("RPC_getIcons", RpcTarget.MasterClient);
	}

	private void UpdateMaster(bool isMasterClient) {
		foreach (Transform playerListTransform in _playerListContainer) {
			playerListTransform.gameObject.GetComponent<PlayerListItem>()
				.UpdateMasterClientOptions(isMasterClient);
		}
	}

	private void UpdateRoomList(List<RoomInfo> rooms) {
		// Destroy all the items in the room list
		foreach (Transform transform in _roomListContainer) {
			Destroy(transform.gameObject);
		}

		// Refresh the room container
		Debug.Log("Rooms open: " + rooms.Count);
		for (int i = 0; i < rooms.Count; i++) {
			// Photon doesn't remove rooms, they store the state of their existence in a list of bools
			if (rooms[i].RemovedFromList) continue;
			else Instantiate(_roomListItemPrefab, _roomListContainer).GetComponent<RoomListItem>().SetUp(rooms[i]);
		}
	}

	// --------------- Helper functions ---------------

	private void disableUsernameErrorText() {
		_usernameErrorText.gameObject.SetActive(false);
	}

	private void disableCreationErrorText() {
		_creationFailedText.gameObject.SetActive(false);
	}

	private bool AllPlayersHaveTeams(Transform playerListContainer) {
		int playersWithoutTeam = 0;
		foreach (Transform playerItemTransform in playerListContainer) {
			PlayerListItem playerListItem = playerItemTransform.gameObject.GetComponent<PlayerListItem>();
			if (playerListItem.GetTeamImage().sprite.name == "no_team_icon") playersWithoutTeam++;
		} return playersWithoutTeam == 0;
	}
}
