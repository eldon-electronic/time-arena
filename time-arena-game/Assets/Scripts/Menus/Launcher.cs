using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
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
	[SerializeField] private TMP_Text _usernameErrorText;
	[SerializeField] private Sprite[] _teamIcons;

	private PhotonView _view;
	private string _userID;
	private Dictionary<string, string> _currentPlayerIcons;

	void Awake()
	{
		Instance = this;
		_view = PhotonView.Get(this);
		_currentPlayerIcons = new Dictionary<string, string>();	
	}

	void Start() {
		if (!PhotonNetwork.IsConnected) {	
			Debug.Log("Connecting to Master");
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	void Update() {
		_startButton.interactable = AllPlayersHaveTeams(_playerListContainer);
	}

	// ---------- PUN Callbacks ----------

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
		_userID = _view.Controller.UserId;

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
			.GetComponent<PlayerListItem>().SetUp(newPlayer, PhotonNetwork.IsMasterClient, _teamIcons);
		_view.RPC("RPC_updateID", newPlayer, newPlayer.UserId);
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

	// ---------- PUBLIC ----------

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

	// Gets run when master client presses start game
	public void StartGame() {
		if (PhotonNetwork.IsMasterClient) {
			_currentPlayerIcons = RefreshIconAssignments();
			Player[] players = PhotonNetwork.PlayerList;
			foreach (Player player in players) {
				_view.RPC("RPC_saveTeam", player, player);
				_view.RPC("RPC_saveIcons", player, _currentPlayerIcons);
			}
		}

		// Level index of PreGameScene is 1, check build settings
		PhotonNetwork.LoadLevel(1);
	}

	public void QuitGame() {
		Application.Quit();
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

	public void OpenChooseTeamMenu() {
		MenuManager.Instance.OpenMenu("chooseTeamMenu");
	}

	public void JoinRoom(RoomInfo info) {
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("loadingMenu");
	}

	public void LeaveRoom() {
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("loadingMenu");
	}

	public void ReturnToMain()
	{
		MenuManager.Instance.OpenMenu("mainMenu");
	}

	// Updates the icon of a given player's list item across the network
	public void UpdateIcon(string userID, string spriteName) {
		_view.RPC("RPC_updateIcon", RpcTarget.All, userID, spriteName);
	}

	// ---------- PRIVATE ----------

	// Helper function to disable no username error text after some time has passed.
	private void disableUsernameErrorText() {
		_usernameErrorText.gameObject.SetActive(false);
	}

	// Helper function to disable text pop-up OnCreateRoomFailed after some time has passed.
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

	// Called when player enters room, sends RPC request to master to retrieve player icons
	// so all players see the correct icon assignments
	private void UpdatePlayerList() {
		foreach (Transform playerListTransform in _playerListContainer) {
			Destroy(playerListTransform.gameObject);
		} _view.RPC("RPC_getIcons", RpcTarget.MasterClient);
	}

	private void UpdateMaster(bool isMasterClient) {
		foreach (Transform playerListTransform in _playerListContainer) {
			playerListTransform.gameObject.GetComponent<PlayerListItem>()
				.UpdateMasterClientOptions(isMasterClient);
		}
	}

	private Dictionary<string, string> RefreshIconAssignments() {
		_currentPlayerIcons.Clear();
		foreach (Transform playerListTransform in _playerListContainer) {
			PlayerListItem playerListItem = playerListTransform.gameObject.GetComponent<PlayerListItem>();
			_currentPlayerIcons.Add(playerListItem.GetUserID(), playerListItem.GetTeamImage().sprite.name);
		} return _currentPlayerIcons;
	}

	// ---------- RPC ----------

	[PunRPC] void RPC_updateIcon(string userID, string spriteName) {
		Sprite newIcon = null;
		foreach (Sprite teamIcon in _teamIcons) {
			if (teamIcon.name == spriteName) newIcon = teamIcon;
		}

		foreach (Transform playerListTransform in _playerListContainer) {
			PlayerListItem playerListItem = playerListTransform.gameObject.GetComponent<PlayerListItem>();
			if (playerListItem.GetUserID() == userID) {
				playerListItem.SetTeamImage(newIcon);
			}
		}
	}

	[PunRPC] void RPC_getIcons() {
		_currentPlayerIcons.Clear();

		foreach (Transform playerListTransform in _playerListContainer) {
			PlayerListItem playerListItem = playerListTransform.gameObject.GetComponent<PlayerListItem>();
			_currentPlayerIcons.Add(playerListItem.GetUserID(), playerListItem.GetTeamImage().sprite.name);
		}

		// If you're the first to enter the room there are no PlayerListItems in the container,
		// then simply add yourself and specify no icon.
		if (_currentPlayerIcons.Count == 0) {
			Player[] players = PhotonNetwork.PlayerList;
			_currentPlayerIcons.Add(players[0].UserId, "no_team_icon");
		}

		// Send out the icons assignment to everyone in the room.
		_view.RPC("RPC_instantiatePlayers", RpcTarget.All, _currentPlayerIcons);
	}

	[PunRPC] void RPC_instantiatePlayers(Dictionary<string, string> playerIcons) {
		Player[] players = PhotonNetwork.PlayerList;
		foreach(Transform playerListTransform in _playerListContainer) {
			Destroy(playerListTransform.gameObject);
		}

		for (int i = 0; i < players.Length; i++) {
			Instantiate(_playerListItemPrefab, _playerListContainer)
				.GetComponent<PlayerListItem>()
				.SetUp(players[i], PhotonNetwork.IsMasterClient, _teamIcons, playerIcons[players[i].UserId]);
		}
	}

	[PunRPC] void RPC_saveIcons(Dictionary<string, string> iconAssignments) {
		foreach (KeyValuePair<string, string> iconAssignment in iconAssignments) {
			PlayerPrefs.SetString(iconAssignment.Key, iconAssignment.Value);
		}
	}

	[PunRPC] void RPC_saveTeam(Player player) {
		foreach (Transform playerListTransform in _playerListContainer) {
			PlayerListItem playerListItem = playerListTransform.gameObject.GetComponent<PlayerListItem>();
			if (player.UserId == playerListItem.GetUserID()) {
				string teamName = playerListItem.GetTeamImage().sprite.name.Split('_')[0];
				PlayerPrefs.SetString("team", teamName);
				Debug.Log($"{player.UserId}: {teamName}");
			}
		}
	}

	[PunRPC] void RPC_updateID(string userID) {
		_userID = userID;
	}
 }

