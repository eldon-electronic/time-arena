using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : SceneController
{
	[SerializeField] private Transform[] _minerSpawnpoints;
	[SerializeField] private Transform[] _guardianSpawnpoints;
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
		_miners = new Dictionary<int, PlayerMinerController>();
		_guardians = new Dictionary<int, PlayerGuardianController>();
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

	public override void Register(PlayerMinerController pmc) {
		base.Register(pmc);
		pmc.SetSpawnpoint(GetSpawnpoint(_minerSpawnpoints));
	}

	public override void Register(PlayerGuardianController pgc) {
		base.Register(pgc);
		pgc.SetSpawnpoint(GetSpawnpoint(_guardianSpawnpoints));
	}

	private void CheckWon()
	{	
		if (_timeLord.TimeEnded() && !_gameEnded)
		{
			_gameEnded = true;
			Constants.Team _winner;
			if(GetMinerScore() > 10) _winner = Constants.Team.Miner;
			else _winner = Constants.Team.Guardian;
			Debug.Log("Game ended");
			gameEnded?.Invoke(_winner);
		}
	}

	private Vector3 GetSpawnpoint(Transform[] spawnpoints) 
	{
		int index = UnityEngine.Random.Range(0, spawnpoints.Length);
		return spawnpoints[index].position;
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
