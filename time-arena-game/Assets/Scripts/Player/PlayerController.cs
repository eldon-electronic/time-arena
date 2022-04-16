using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
	[SerializeField] private Camera Cam;
	[SerializeField] private Canvas UI;
	[SerializeField] private PhotonView _view;
	public Constants.Team Team;
	public int ID;


	// ------------ UNITY METHODS ------------

	void Awake()
	{
		ID = _view.ViewID;

		// TODO: Set the team in the menu before loading the pregame scene.
		if (ID == 1001) Team = Constants.Team.Guardian;
		else Team = Constants.Team.Miner;
	}

	void OnEnable() { GameController.gameActive += OnGameActive; }

	void OnDisable() { GameController.gameActive -= OnGameActive; }

	void Start()
	{	
		DontDestroyOnLoad(gameObject);

		gameObject.layer = Constants.LayerPlayer;

		SceneController sceneController = FindObjectOfType<PreGameController>();
		if (sceneController == null) Debug.LogError("PreGameController not found");
		else sceneController.Register(this);
		
		if (_view.IsMine) gameObject.tag = "Client";
		else
		{
			Destroy(Cam.gameObject);
			Destroy(UI.gameObject);
		}

		// Allow master client to move players from one scene to another.
        PhotonNetwork.AutomaticallySyncScene = true;

		// Lock players cursor to center screen.
        Cursor.lockState = CursorLockMode.Locked;
	}


	// ------------ PRIVATE METHODS ------------

	private void OnGameActive(GameController game)
	{
		game.Register(this);
		gameObject.layer = Constants.LayerPlayer;
	}


	// ------------ PUBLIC METHODS ------------

	public void Show() { gameObject.layer = Constants.LayerPlayer; }

	public void Hide() { gameObject.layer = Constants.LayerOutsideReality; }
}
