using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviour
{
	private Dictionary<int, PlayerController> _miners;
	private Dictionary<int, PlayerController> _guardians;
	private TimeLord _timeLord;

	public float Timer;
	public bool GameStarted = false;
	public bool GameEnded = false;
	public Constants.Team WinningTeam = Constants.Team.Miner;
	private int _minerScore;


	void Start()
	{
		// Prevent anyone else from joining room.
		PhotonNetwork.CurrentRoom.IsOpen = false;

		_miners = new Dictionary<int, PlayerController>();
		_guardians = new Dictionary<int, PlayerController>();

		Timer = 5f;
		_minerScore = 0;

		int totalFrames = Constants.GameLength * Constants.FrameRate;
		_timeLord = new TimeLord(totalFrames);

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		GameObject client = GameObject.FindWithTag("Client");

		List<GameObject> allPlayers = new List<GameObject>(players);
		allPlayers.Add(client);

		foreach (var player in allPlayers)
		{
			PlayerController pc = player.GetComponent<PlayerController>();
			pc.SetGame(this);
			pc.SetTimeLord(_timeLord);

			int id = pc.GetID();
			if (pc.Team == Constants.Team.Guardian) _guardians.Add(id, pc);
			else _miners.Add(id, pc);
		}
	}


	// ------------ UPDATE HELPER FUNCTIONS ------------

	// Checks to see if there are no hiders left.
	private void CheckWon()
	{	
		if (_timeLord.TimeEnded() && !GameEnded)
		{
			GameEnded = true;
			// TODO: Add a check to see who actually won based on whether the miners reached their target.
			WinningTeam = Constants.Team.Miner;
		}
	}

	void Update()
	{
		if (!GameStarted)
		{
			// Pregame timer is counting.
			if (Timer <= 0f)
			{
				if (!GameStarted) GameStarted = true;
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

	public void HideAllPlayers()
	{
		foreach (var guardian in _guardians)
		{
			guardian.Value.Hide();
		}
		foreach (var miner in _miners)
		{
			miner.Value.Hide();
		}
	}

	public void ShowPlayersInReality()
	{
		HashSet<int> playerIDs = _timeLord.GetPlayersInReality();
		foreach (var id in playerIDs)
		{
			if (_guardians.ContainsKey(id)) _guardians[id].Show();
			else if (_miners.ContainsKey(id)) _miners[id].Show();
		}
	}

	public void IncrementMinerScore() { _minerScore++; }

	public int GetMinerScore() { return _minerScore; }
}
