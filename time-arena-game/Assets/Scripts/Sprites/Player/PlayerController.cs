using Photon.Pun;
using System;
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
	protected SceneController _sceneController;
	public Constants.Team Team;
	public int ID;
	public int Score;
	public static event Action<PlayerController> clientEntered;


	// ------------ UNITY METHODS ------------

	void Awake()
	{
		ID = _view.ViewID;
		//SetActive();
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

		if (_view.IsMine) clientEntered?.Invoke(this);
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

	void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		int channelID = (int) info.photonView.InstantiationData[0];
		Debug.Log($"channel ID: {channelID}");
	}


	// ------------ PRIVATE METHODS ------------

	private void OnGameActive(GameController game)
	{
		_sceneController = game;
		_sceneController.Register(this);
		Show();
		Score = 0;
		if (_view.IsMine) clientEntered?.Invoke(this);
	}

	public abstract void SetActive(bool _isPreGame);

    protected abstract void SetTeam();

    public abstract void SetSceneController(SceneController sceneController);

	public abstract void IncrementScore();


	// ------------ PUBLIC METHODS ------------

	public Dictionary<int, string> GetViewIDTranslation() {
		_viewIDtoUserID = new Dictionary<int, string>();
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (var player in players) {
			PhotonView playerView = player.GetComponent<PhotonView>();
			string playerRealtimeID = playerView.Owner.UserId;
			_viewIDtoUserID.Add(playerView.ViewID, playerRealtimeID);
		} _userID = _viewIDtoUserID[ID];
		return _viewIDtoUserID;
	}

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


	// ------------ RPC METHODS ------------

	[PunRPC]
	public void RPC_incrementScore() { Score++; }
}
