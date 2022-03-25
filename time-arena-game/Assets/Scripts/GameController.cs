using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviour
{
	private int _totalFrames;
	private int _currentFrame;
	private List<int> _miners;
	private List<int> _guardians;
	private RealityManager _realities;
	private List<PlayerState>[] _playerStates;
	[SerializeField] private GameObject _tailPrefab;
	private Dictionary<int, TailController> _tails;
	private int _myID;
	private float _timer;


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

		_timer = 0f;
		_ended = false;

		if (PhotonNetwork.IsMasterClient) SetupNewGame();
	}


	// ------------ UPDATE FUNCTIONS ------------


	// Checks to see if there are no hiders left.
	private void CheckWon()
	{
		if (_miners.Count == 0)
		{
			// Code reaches here even though hiders are remaining.
			GameEnded = true;
			WinningTeam = Constants.Team.Guardian;
		}
		
		if (_currentFrame >= Constants.GameLength && !GameEnded)
			{
				GameEnded = true;
				WinningTeam = Constants.Team.Miner;
			}
	}


	void Update()
	{
		if (!GameStarted)
		{
			// Pregame timer is counting.
			if (_timer >= 5f) GameStarted = true;
			else _timer += Time.deltaTime;
		}
		else
		{
			// Increment global frame and individual player frames.
			if (!GameEnded)
			{
				_currentFrame++;
				_realities.Increment();

				int myFrame = _realities.GetPerceivedFrame(_myID);
				
				// This handles the state of tails in your reality.
				foreach (var state in _playerStates[myFrame])
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
			}
			CheckWon();
		}
	}


	// ------------ PUBLIC FUNCTIONS ------------


	public void Connect(int playerID, bool isMe)
	{
		_realities.AddHead(playerID);
		if (isMe) _myID = playerID;
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
		else if (frame <= Constants.MinTimeSnapDistance)
		{
			frame = 0;
		}
		else if (frame + Constants.MinTimeSnapDistance >= _currentFrame)
		{
			frame = _currentFrame;
		}
		_realities.AddTail(playerID, frame);
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
		int frame = _realities.GetPerceivedFrame(_myID);

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

	// TODO: adapt so it takes in a Constants.Team as parameter
	public List<float> GetPlayerPositions()
	{
		List<float> positions = new List<float>();

		List<(int id, int frame)> players = _realities.GetPerceivedFrames();
		foreach (var player in players)
		{
			if (player.id != _myID)
			{
				float position = (float) player.frame / (float) _totalFrames;
				positions.Add(position);
			}
		}

		return positions;
	}

	public float GetYourPosition()
	{
		int frame = _realities.GetPerceivedFrame(_myID);
		float position = (float) frame / (float) _totalFrames;
		return position;
	}

	// Returns the fraction through the game time.
	public float GetTimeProportion()
	{
		return (float) _currentFrame / (float) _totalFrames;
	}

	// Returns the elapsed time in seconds.
	public int GetElapsedTime()
	{
		return _currentFrame / Constants.FrameRate;
	}

	public bool CanJump(int playerID, Constants.JumpDirection direction)
	{
		int frame = _realities.GetPerceivedFrame(playerID);
		if (direction == Constants.JumpDirection.Backward)
		{
			return frame - Constants.TimeTravelVelocity >= 0;
		}
		else
		{
			return frame + Constants.TimeTravelVelocity <= _currentFrame;
		}
	}

	public void SetTeam(int playerID, Constants.Team team)
	{
		if (team == Constants.Team.Guardian)
		{
			_miners.Remove(playerID);
			_guardians.Add(playerID);
		}
		else if (team == Constants.Team.Miner)
		{
			_guardians.Remove(playerID);
			_miners.Add(playerID);
		}
	}
}
