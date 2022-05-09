using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class PlayerController : MonoBehaviour, Debuggable, IPunInstantiateMagicCallback
{
	[SerializeField] protected GameObject _camera;
	[SerializeField] protected GameObject _UI;
	[SerializeField] protected GameObject _me;
	[SerializeField] protected PhotonView _view;
	[SerializeField] protected GameObject _mesh;
	[SerializeField] private HudMasterClientOptions _hudMasterClientOptions;
	protected SceneController _sceneController;
	protected string _userID;
	protected Dictionary<int, string> _viewIDtoUserID;
	protected int _channelID;
	public Constants.Team Team;
	public int ID;
	public int Score;
	public static event Action<PlayerController> clientEntered;


	// ------------ UNITY METHODS ------------

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		_channelID = (int) info.photonView.InstantiationData[0];
	}

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

		PreGameController pregame = FindObjectOfType<PreGameController>();
		pregame.Register(this);
		Transform channels = pregame.GetChannels();

		FindObjectOfType<HudDebugPanel>().Register(this);

		if (_view.IsMine)
		{
			clientEntered?.Invoke(this);
			for (int i=0; i < channels.childCount; i++)
			{
				if (i != _channelID) Destroy(channels.GetChild(i).Find("TutorialCamera").gameObject);
			}
		}
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

	void OnTriggerEnter(Collider collider)
	{
		if (_view.IsMine && PhotonNetwork.IsMasterClient && collider.gameObject.tag == "CentralTutorialGround")
		{
			((PreGameController) _sceneController).SetCanStart(true);
			_hudMasterClientOptions.Show();
		}
	}


	// ------------ PRIVATE METHODS ------------

	private void OnGameActive(GameController game)
	{
		game.Register(this);
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

	public virtual void Show()
	{
		if (!_view.IsMine)
		{
			gameObject.layer = Constants.LayerPlayer;
			_mesh.SetActive(true);
		}
	}

	public virtual void Hide()
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

	public void Synchronise(Dictionary<int, int[]> data, int frame)
	{
		_view.RPC("RPC_synchronise2", RpcTarget.All, data, frame);
	}


	// ------------ RPC METHODS ------------

	[PunRPC]
	public void RPC_incrementScore() { Score++; }
}
