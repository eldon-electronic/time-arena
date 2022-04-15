using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : SceneController
{
	public float Timer;
	private bool _gameStarted;
	private bool _gameEnded;
	public static event Action<GameController> gameActive;
	public static event Action gameStarted;
	public static event Action<Constants.Team> gameEnded;
	public static event Action<TimeLord> newTimeLord;


	void Awake()
	{
		_miners = new Dictionary<int, PlayerController>();
		_guardians = new Dictionary<int, PlayerController>();

		Timer = 5f;
		_gameStarted = false;
		_gameEnded = false;
		_minerScore = 0;
	}


	void Start()
	{
		// Prevent anyone else from joining room.
		PhotonNetwork.CurrentRoom.IsOpen = false;

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		GameObject client = GameObject.FindWithTag("Client");

		List<GameObject> allPlayers = new List<GameObject>(players);
		allPlayers.Add(client);

		gameActive?.Invoke(this);

		_timeLord = new TimeLord(Constants.GameLength * Constants.FrameRate);
		newTimeLord?.Invoke(_timeLord);

		foreach (var player in allPlayers)
		{
			PlayerController pc = player.GetComponent<PlayerController>();
			int id = pc.GetID();
			if (pc.Team == Constants.Team.Guardian) _guardians.Add(id, pc);
			else _miners.Add(id, pc);
		}
	}

	private void CheckWon()
	{	
		if (_timeLord.TimeEnded() && !_gameEnded)
		{
			_gameEnded = true;
			// TODO: Add a check to see who actually won based on whether the miners reached their target.
			gameEnded?.Invoke(Constants.Team.Miner);
		}
	}

	void Update()
	{
		if (!_gameStarted)
		{
			// Pregame timer is counting.
			if (Timer <= 0f)
			{
				if (!_gameStarted)
				{
					_gameStarted = true;
					gameStarted?.Invoke();
				}
			}
			else Timer -= Time.deltaTime;
		}
		else
		{
			// Increment global frame and individual player frames.
			if (!_gameEnded) _timeLord.Tick();
			CheckWon();
		}
	}


	// ------------ PUBLIC FUNCTIONS ------------

	// TODO: remove this.
	public void SetTeam(int playerID, Constants.Team team)
	{
		if (team == Constants.Team.Guardian)
		{
			PlayerController player = _miners[playerID];
			_miners.Remove(playerID);
			_guardians.Add(playerID, player);
		}
		else if (team == Constants.Team.Miner)
		{
			PlayerController player = _guardians[playerID];
			_guardians.Remove(playerID);
			_miners.Add(playerID, player);
		}
	}
}
