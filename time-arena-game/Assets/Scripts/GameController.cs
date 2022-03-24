using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviour
{
	private int _totalFrames;
	private int _currentFrame;
	private RealityManager _realities;
	private List<PlayerState>[] _playerStates;
	[SerializeField] private GameObject _tailPrefab;
	private Dictionary<int, TailController> _tails;
	private int _masterClient;

    // Variables referring to the game state.
	// 5 minute rounds * sixty seconds.
    public float GameLength = 5f * 60f;
    public float TimeElapsedInGame = 0f;

	private PlayerController _client;
	public List<PlayerController> _players;

	// List to keep track of elapsed time for all players.
  	public Dictionary<int, float> OtherPlayersElapsedTime = new Dictionary<int, float>();

	public bool GameStarted = false;
	public bool GameEnded = false;
	public Constants.Team WinningTeam = Constants.Team.Miner;


	void Start()
	{
		// Prevent anyone else from joining room.
		PhotonNetwork.CurrentRoom.IsOpen = false;

		_totalFrames = Constants.GameLength * Constants.FrameRate;
		_currentFrame = 0;

		_playerStates = new List<PlayerState>[_totalFrames];
		_realities = new RealityManager();

		_tails = new Dictionary<int, TailController>();

		if (PhotonNetwork.IsMasterClient) SetupNewGame();
	}

	// TODO: Revise this ground up.
	void Update()
	{
		// Increment global timer and individual player timers.
		if (!GameEnded)
		{
			_currentFrame++;
			_realities.Increment();

			int masterClientFrame = _realities.GetPerceivedFrame(_masterClient);
			
			// This handles the state of tails in your reality.
			foreach (var state in _playerStates[masterClientFrame])
			{
				int id = state.TailID;

				if (!_tails.ContainsKey(id))
				{
					// A tail enters your reality.
					GameObject tail = (GameObject) Resources.Load("rePlayer");
					TailController tailController = tail.GetComponent<TailController>();
					tailController.Initialise(state);
					_tails.Add(id, tailController);
				}
				else _tails[id].SetState(state);

				if (state.Kill)
				{
					// A tail leaves your reality.
					_tails[id].Destroy();
					_tails.Remove(id);
				}
			}

			// TODO: Remove this and refactor dependent code.
			TimeElapsedInGame += Time.deltaTime;
			List<int> keys = new List<int>(OtherPlayersElapsedTime.Keys);
			foreach (int key in keys)
			{
				OtherPlayersElapsedTime[key] += Time.deltaTime / GameLength;
			}
		}

		// If pregame timer is counting.
		if (!GameStarted)
		{
			if (TimeElapsedInGame >= 5f)
			{
				GameStarted = true;
				TimeElapsedInGame = 0f;
				List<int> keys = new List<int>(OtherPlayersElapsedTime.Keys);
				foreach (int key in keys)
				{
					OtherPlayersElapsedTime[key] = 0f;
				}
			}
		}
		// Else game is in play.
		else
		{
			CheckHidersLeft();
			if (TimeElapsedInGame >= GameLength && !GameEnded)
			{
				GameEnded = true;
				WinningTeam = Constants.Team.Miner;
				_client.OnGameEnded();
			}
		}
	}


	// ------------ PUBLIC FUNCTIONS ------------


	public void Connect(int playerID, bool isMasterClient)
	{
		_realities.AddHead(playerID);
		if (isMasterClient) _masterClient = playerID;
	}

	public void RecordState(PlayerState ps)
	{
		int lastTailID = _realities.GetLastTailID(ps.PlayerID);
		List<int> frames = _realities.GetTailFrames(ps.PlayerID);
		for (int i=0; i < frames.Count; i++)
		{
			ps.TailID = lastTailID + i;
			int frame = frames[i];
			_playerStates[frame].Add(ps);
		}
	}

	public void TimeTravel(int playerID, Constants.JumpDirection jd)
	{
		int offset = (jd == Constants.JumpDirection.Forward) ? Constants.TimeTravelVelocity : - Constants.TimeTravelVelocity;
		_realities.OffsetPerceivedFrame(playerID, offset);
	}

	// TODO: Think more carefully about the 3 or 4 cases when you leave and enter in a different order.
	public void LeaveReality(int playerID)
	{
		_realities.RemoveTail(playerID);
	}

	// Snap to the nearest reality within range, else create a new reality.
	// Return a list of tails that exist here
	public void EnterReality(int playerID)
	{
		// Start recording the player in the new reality.
		int frame = _realities.GetPerceivedFrame(playerID);
		int closestFrame = _realities.GetClosestFrame(playerID, frame);
		if (Mathf.Abs(closestFrame - frame) < Constants.MinTimeSnapDistance)
		{
			frame = closestFrame;
		}
		_realities.AddTail(playerID, frame);
		
		// Add tails to the game.

	}

	public void DestroyTails()
	{
		foreach (var tail in _tails)
		{
			tail.Value.Destroy();
		}
		_tails = new Dictionary<int, TailController>();
	}

	public void BirthTails()
	{
		int frame = _realities.GetPerceivedFrame(_masterClient);

		foreach (List<PlayerState> tailStates in _playerStates)
		{
			foreach (PlayerState tailState in tailStates)
			{
				GameObject tail = (GameObject) Resources.Load("rePlayer");
				TailController tailController = tail.GetComponent<TailController>();
				tailController.Initialise(tailState);
				_tails.Add(tailState.TailID, tailController);
			}
		}
	}
















	// ------------ START HELPER FUNCTIONS ------------

	private void AddClient()
	{
		GameObject[] clients = GameObject.FindGameObjectsWithTag("Client");
		if (clients.Length == 1)
		{
			_client = clients[0].GetComponent<PlayerController>();
			_players.Add(_client);
			OtherPlayersElapsedTime.Add(_client.View.ViewID, 0f);
			// _client._game = this;
		}
		else Debug.LogError("No master client");
	}

	private void AddPlayers()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in players)
		{
			PlayerController playerComponent = player.GetComponent<PlayerController>();
			_players.Add(playerComponent);
			OtherPlayersElapsedTime.Add(playerComponent.View.ViewID, 0f);
		}
	}

    // Initialise teams and spawn locations for the new game.
	private void SetupNewGame()
	{
		// If testing with one player, they are hider, otherwise one player will randomly be seeker.
		if (_players.Count > 1)
		{
			int randomIndex = Random.Range(0, _players.Count - 1); 
			_players[randomIndex].GetFound();
		}
	}

	// Checks to see if there are no hiders left.
	public void CheckHidersLeft()
	{
		bool isHidersRemaining = false;
		for (int i = 0; i < _players.Count; i++)
		{
			isHidersRemaining |= (_players[i].Team == Constants.Team.Miner);
		}
		if (!isHidersRemaining)
		{
			// Code reaches here even though hiders are remaining.
			GameEnded = true;
			WinningTeam = Constants.Team.Guardian;
			_client.OnGameEnded();
		}
	}
}
