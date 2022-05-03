using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class PlayerController : MonoBehaviour, Debuggable
{
	[SerializeField] protected GameObject _camera;
	[SerializeField] protected GameObject _UI;
	[SerializeField] protected GameObject _me;
	[SerializeField] protected PhotonView _view;
	[SerializeField] protected GameObject _mesh;
	private string _userID;
	private Dictionary<int, string> _viewIDtoUserID;
	private Dictionary<int, string> _iconAssignments;
	protected SceneController _sceneController;
	public Constants.Team Team;
	public int ID;


	// ------------ UNITY METHODS ------------

	void Awake()
	{
		ID = _view.ViewID;
		SetActive();
        SetTeam();
	}

	void OnEnable() { GameController.gameActive += OnGameActive; }

	void OnDisable() { GameController.gameActive -= OnGameActive; }

	void Start()
	{
		DontDestroyOnLoad(gameObject);

		gameObject.layer = Constants.LayerPlayer;

		FindObjectOfType<PreGameController>().Register(this);

		FindObjectOfType<HudDebugPanel>().Register(this);

		if (_view.IsMine) gameObject.tag = "Client";
		else
		{
			Destroy(_camera);
			Destroy(_UI);
			Destroy(_me);
		}

		// Allow master client to move players from one scene to another.
        PhotonNetwork.AutomaticallySyncScene = true;

		// Lock players cursor to center screen.
        Cursor.lockState = CursorLockMode.Locked;
	}


	// ------------ PRIVATE METHODS ------------

	private void OnGameActive(GameController game)
	{
		_sceneController = game;
		_sceneController.Register(this);
		Show();
	}

	private void GetIconAssignment() {
		_viewIDtoUserID = new Dictionary<int, string>();
		_iconAssignments = new Dictionary<int, string>();

		// Translation from Photon's View IDs to Photon's Realtime UserIds
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (var player in players) {
			PhotonView playerView = player.GetComponent<PhotonView>();
			string playerRealtimeID = playerView.Owner.UserId;
			_viewIDtoUserID.Add(playerView.ViewID, playerRealtimeID);
		} _userID = _viewIDtoUserID[ID];

		// Getting icon assignment from saved PlayerPrefs
		foreach (KeyValuePair<int, string> pair in _viewIDtoUserID) {
			_iconAssignments.Add(pair.Key, PlayerPrefs.GetString(pair.Value));
		}
	}

	protected abstract void SetActive();

    protected abstract void SetTeam();

    public abstract void SetSceneController(SceneController sceneController);


	// ------------ PUBLIC METHODS ------------

	public void Show()
	{
		if (!_view.IsMine)
		{
			gameObject.layer = Constants.LayerPlayer;
			_mesh.SetActive(true);
		}
	}

	public void Hide()
	{
		if (!_view.IsMine)
		{
			gameObject.layer = Constants.LayerOutsideReality;
			_mesh.SetActive(false);
		}
	}

	public Hashtable GetDebugValues()
	{
		Hashtable debugValues = new Hashtable();
		debugValues.Add($"{_view.ViewID} layer", gameObject.layer);
		return debugValues;
	}

	public string GetTeamName() {
		string icon = _iconAssignments[ID];
		return icon.Split('-')[0];
	}
}
