using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviour
{
	private Dictionary<int, PlayerController> _miners;
	private Dictionary<int, PlayerController> _guardians;
	private TimeLord _timeLord;
	private Dictionary<int, TailController> _tails;
	private float _timer;

	public bool GameStarted = false;
	public bool GameEnded = false;
	public Constants.Team WinningTeam = Constants.Team.Miner;


	void Start()
	{
		// Prevent anyone else from joining room.
		PhotonNetwork.CurrentRoom.IsOpen = false;

		_miners = new Dictionary<int, PlayerController>();
		_guardians = new Dictionary<int, PlayerController>();
		_tails = new Dictionary<int, TailController>();

		_timer = 0f;
	}


	// ------------ UPDATE HELPER FUNCTIONS ------------

	// Checks to see if there are no hiders left.
	private void CheckWon()
	{
		if (_miners.Count == 0)
		{
			// Code reaches here even though hiders are remaining.
			GameEnded = true;
			WinningTeam = Constants.Team.Guardian;
		}
		
		if (_timeLord.TimeEnded() && !GameEnded)
			{
				GameEnded = true;
				WinningTeam = Constants.Team.Miner;
			}
	}

	private void SetupTimeLord()
	{
		int totalFrames = Constants.GameLength * Constants.FrameRate;
		_timeLord = new TimeLord(totalFrames);
		foreach (var miner in _miners)
		{
			miner.Value.SetTimeLord(_timeLord);
		}
		foreach (var guardian in _guardians)
		{
			guardian.Value.SetTimeLord(_timeLord);
		}
	}

	void Update()
	{
		if (!GameStarted)
		{
			// Pregame timer is counting.
			if (_timer >= 5f)
			{
				if (!GameStarted)
				{
					GameStarted = true;
					SetupTimeLord();
				}
			}
			else _timer += Time.deltaTime;
		}
		else
		{
			// Increment global frame and individual player frames.
			if (!GameEnded) _timeLord.Tick();
			CheckWon();
		}
	}


	// ------------ PUBLIC FUNCTIONS ------------

	public void Register(int id, PlayerController pc, Constants.Team team)
	{
		if (team == Constants.Team.Guardian) _guardians.Add(id, pc);
		else _miners.Add(id, pc);
	}

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
