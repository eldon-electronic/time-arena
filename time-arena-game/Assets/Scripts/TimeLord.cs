using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLord
{
    private int _totalFrames;
	private int _currentFrame;
    private int _myID;

    // A RealityManager object for keeping track of each individual's current frames.
    private RealityManager _realities;

    // An array the lenth of the game, with an item for each frame.
    // Each item stores a dictionary that maps tailIDs to their state.
    private Dictionary<int, PlayerState>[] _playerStates;

    // TODO: Improve this to be neater; at the moment it doesn't help when time travelling, only when moving normally.
    // A dictionary that maps frames to a list of tailIDs for those tails that were created on this frame.
    private Dictionary<int, List<int>> _tailCreations;


    public TimeLord(int totalFrames)
    {
        _totalFrames = totalFrames;
		_currentFrame = 0;

		_playerStates = new Dictionary<int, PlayerState>[_totalFrames];
		_realities = new RealityManager();
        _tailCreations = new Dictionary<int, List<int>>();
    }


    // ------------ PUBLIC METHODS FOR THE GAME CONTROLLER ------------

    // Increments game time as well as the individual time for all player realities.
    public void Tick()
    {
        _currentFrame++;
        _realities.Increment();
    }

    // Returns a list of states for all tails created on your perceived frame.
    public List<PlayerState> GetCreatedTails()
    {
        int frame = _realities.GetPerceivedFrame(_myID);
        if (_tailCreations.ContainsKey(frame))
        {
            List<PlayerState> result = new List<PlayerState>();

            List<int> tailIDs = _tailCreations[frame];
            foreach (var id in tailIDs)
            {
                result.Add(_playerStates[frame][id]);
            }

            return result;
        }
        else return null;
    }

    // Returns the states of all tails that exist on your perceived frame.
    public Dictionary<int, PlayerState> GetAllTails()
    {
        int frame = _realities.GetPerceivedFrame(_myID);
        return _playerStates[frame];
    }

    public bool TimeEnded() { return _currentFrame >= _totalFrames; }


    // ------------ PUBLIC METHODS FOR THE TAIL CONTROLLER ------------

    public PlayerState GetState(int tailID)
    {
        int frame = _realities.GetPerceivedFrame(_myID);
        if (_playerStates[frame].ContainsKey(_myID)) return _playerStates[frame][_myID];
        else return null;
    }


    // ------------ PUBLIC METHODS FOR THE PLAYER CONTROLLER ------------

    // Adds the given player to the Reality Manager, allowing them to time travel.
    public void Connect(int playerID, bool isMe)
	{
		_realities.AddHead(playerID);
		if (isMe) _myID = playerID;
	}

    // Records the given state in all realities this player exists in.
	public void RecordState(PlayerState ps)
	{
		int lastTailID = _realities.GetLastTailID(ps.PlayerID);
		List<int> frames = _realities.GetTailFrames(ps.PlayerID);
		for (int i=0; i < frames.Count; i++)
		{
			ps.TailID = lastTailID + i;
			int frame = frames[i];
			_playerStates[frame].Add(ps.TailID, ps);
		}
	}

    // Makes the given player's perceived time jump in the given direction.
	public void TimeTravel(int playerID, Constants.JumpDirection jd)
	{
		int offset = (jd == Constants.JumpDirection.Forward) ? Constants.TimeTravelVelocity : - Constants.TimeTravelVelocity;
		_realities.OffsetPerceivedFrame(playerID, offset);
	}

    // Stops recording in your previous reality.
	public void LeaveReality(int playerID)
	{
		_realities.RemoveTail(playerID);
	}

	// Snaps your position to the nearest reality within range, else creates a new reality.
    // Starts recording in this new reality.
	public void EnterReality(int playerID)
	{
		// Snap to the closest frame.
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

        // Start recording in the new reality.
        _realities.AddTail(playerID, frame);

        // Record the frame at which this tail was created.
        int tailID = _realities.GetNextTailID(playerID);
        if (_tailCreations.ContainsKey(frame))
        {
            List<int> tailIDs = _tailCreations[frame];
            tailIDs.Add(tailID);
            _tailCreations[frame] = tailIDs;
        }
        else _tailCreations.Add(frame, new List<int>(){tailID});
	}

    // TODO: adapt so it takes in a Constants.Team as parameter
    // Returns the positions of all players (except you) as a fraction through the game time.
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

    // Returns your position in time as a fraction through the game time. 
	public float GetYourPosition()
	{
		int frame = _realities.GetPerceivedFrame(_myID);
		float position = (float) frame / (float) _totalFrames;
		return position;
	}

	// Returns the fraction elapsed through the game time.
	public float GetTimeProportion()
	{
		return (float) _currentFrame / (float) _totalFrames;
	}

	// Returns the elapsed time in seconds.
	public int GetElapsedTime()
	{
		return _currentFrame / Constants.FrameRate;
	}

    // Returns true if the given player can travel in the given direction.
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

    // WARNING: This function is to be used by test framework only.
    public Dictionary<int, PlayerState>[] RevealPlayerStates()
    {
        return _playerStates;
    }
}
