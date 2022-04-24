using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private GameObject _camera;
	[SerializeField] private GameObject _UI;
	[SerializeField] private PhotonView _view;
	[SerializeField] private GameObject _miner;
    [SerializeField] private GameObject _guardian;
	[SerializeField] private GameObject _minerBody;

	public Constants.Team Team;
	public int ID;
	public int score;


	// ------------ UNITY METHODS ------------

	void Awake()
	{
		ID = _view.ViewID;

		// TODO: Set the team in the menu before loading the pregame scene.
		if (ID == 2001) Team = Constants.Team.Guardian;
		else Team = Constants.Team.Miner;

		SetCharacter();
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
			Destroy(_camera);
			Destroy(_UI);
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

	private void SetCharacter()
	{
        if (Team == Constants.Team.Guardian)
		{
			if(_view.IsMine){
				_guardian.SetActive(false);
				_miner.SetActive(false);
			}
			else{
				_guardian.SetActive(true);
				_miner.SetActive(false);
			}
           
        }
        else if (Team == Constants.Team.Miner)
		{
			if(_view.IsMine){
				_guardian.SetActive(false);
				_minerBody.SetActive(false);
			}
			else{
				_guardian.SetActive(false);
                _miner.SetActive(true);
			}
            
        }
    }


	// ------------ PUBLIC METHODS ------------

	public void Show()
	{
		gameObject.layer = Constants.LayerPlayer;
		if (Team == Constants.Team.Guardian)
		{
			_guardian.SetActive(true);
		}
		else _miner.SetActive(true);
	}

	public void Hide()
	{
		gameObject.layer = Constants.LayerOutsideReality;
		if (Team == Constants.Team.Guardian)
		{
			_guardian.SetActive(false);
		}
		else _miner.SetActive(false);
	}
}
