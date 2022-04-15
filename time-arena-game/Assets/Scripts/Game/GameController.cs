using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : SceneController
{
	public float Timer;
	private bool _gameStarted;
	public bool GameEnded;
	public Constants.Team WinningTeam;
	public static event Action<GameController> gameActive;
	public static event Action gameStarted;
	public static event Action gameEnded;


	void Awake()
	{
		_miners = new Dictionary<int, PlayerController>();
		_guardians = new Dictionary<int, PlayerController>();

		int totalFrames = Constants.GameLength * Constants.FrameRate;
		_timeLord = new TimeLord(totalFrames);

		Timer = 5f;
		_gameStarted = false;
		GameEnded = false;
		WinningTeam = Constants.Team.Miner;
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

		foreach (var player in allPlayers)
		{
			PlayerController pc = player.GetComponent<PlayerController>();
			pc.SetTimeLord(_timeLord);

			int id = pc.GetID();
			if (pc.Team == Constants.Team.Guardian) _guardians.Add(id, pc);
			else _miners.Add(id, pc);
		}
	}

	private void CheckWon()
	{	
		if (_timeLord.TimeEnded() && !GameEnded)
		{
			GameEnded = true;
			gameEnded?.Invoke();
			// TODO: Add a check to see who actually won based on whether the miners reached their target.
			WinningTeam = Constants.Team.Miner;
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
			if (!GameEnded) _timeLord.Tick();
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
