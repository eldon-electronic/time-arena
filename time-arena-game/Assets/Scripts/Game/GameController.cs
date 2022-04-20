using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : SceneController
{
	private float _timer;
	private bool _gameStarted;
	private bool _gameEnded;
	public static event Action<GameController> gameActive;
	public static event Action gameStarted;
	public static event Action<Constants.Team> gameEnded;
	public static event Action<float> countDown;
	public static event Action<TimeLord> newTimeLord;


	void Awake()
	{
		_miners = new Dictionary<int, PlayerController>();
		_guardians = new Dictionary<int, PlayerController>();
		_timeLord = new TimeLord(Constants.GameLength * Constants.FrameRate);

		_timer = 5f;
		_gameStarted = false;
		_gameEnded = false;
		_minerScore = 0;
	}

	void Start()
	{
		// Prevent anyone else from joining room.
		PhotonNetwork.CurrentRoom.IsOpen = false;

		Debug.Log("New TimeLord");
		newTimeLord?.Invoke(_timeLord);

		Debug.Log("Game Active");
		gameActive?.Invoke(this);
	}

	private void CheckWon()
	{	
		if (_timeLord.TimeEnded() && !_gameEnded)
		{
			_gameEnded = true;
			// TODO: Add a check to see who actually won based on whether the miners reached their target.
			Debug.Log("Game ended");
			gameEnded?.Invoke(Constants.Team.Miner);
		}
	}

	void Update()
	{
		if (!_gameStarted)
		{
			// Game starts.
			if (_timer <= 0f)
			{
				_gameStarted = true;
				Debug.Log("Game started");
				gameStarted?.Invoke();
			}
			// Countdown timer is counting.
			else
			{
				_timer -= Time.deltaTime;
				countDown?.Invoke(_timer);
			}
		}
		else
		{
			// Increment global frame and individual player frames.
			if (!_gameEnded) _timeLord.Tick();
			CheckWon();
		}
	}
}
