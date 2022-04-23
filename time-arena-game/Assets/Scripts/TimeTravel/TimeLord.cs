using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class TimeLord: Debuggable
{
    protected int _totalFrames;
	protected int _currentFrame;
    protected int _myID;

    // A RealityManager object for keeping track of each individual's current frames.
    protected RealityManager _realities;

    // An array the lenth of the game, with an item for each frame.
    // Each item stores a dictionary that maps tailIDs to their state.
    protected Dictionary<int, PlayerState>[] _playerStates;


    public TimeLord(int totalFrames)
    {
        _totalFrames = totalFrames;
		_currentFrame = 0;

		_playerStates = new Dictionary<int, PlayerState>[_totalFrames];
		_realities = new RealityManager();
    }


	// ------------ IMPLEMENTED INTERFACE METHODS ------------

	public Hashtable GetDebugValues()
	{
		Hashtable debugItems = new Hashtable();
		List<(int id, int frame)> frames = _realities.GetPerceivedFrames();
		foreach (var f in frames)
		{
			if (f.id == _myID)
			{
				debugItems.Add("My frame", f.frame);
			}
			else debugItems.Add($"{f.id}'s frame", f.frame);
		}
		debugItems.Add("Current frame", _currentFrame);
		return debugItems;
	}


    // ------------ PUBLIC METHODS FOR THE GAME CONTROLLER ------------

    // Increments game time as well as the individual time for all player realities.
    public virtual void Tick()
    {
		if (!TimeEnded())
		{
			_currentFrame++;
			_realities.Tick();
		}
    }

    public bool TimeEnded() { return _currentFrame >= _totalFrames - 1; }


    // ------------ PUBLIC METHODS FOR TAIL MANAGER ------------

	public Dictionary<int, PlayerState> GetTailStates()
	{
		int frame = _realities.GetPerceivedFrame(_myID);
		if (_playerStates[frame] != null)
		{
			return _playerStates[frame];
		}
		return new Dictionary<int, PlayerState>();
	}


    // ------------ PUBLIC METHODS FOR THE TAIL CONTROLLER ------------

	// Returns the state for the given tail at your current perceived frame.
    public PlayerState GetTailState(int tailID)
    {
        int frame = _realities.GetPerceivedFrame(_myID);
        if (_playerStates[frame] != null)
        {
            if (_playerStates[frame].ContainsKey(tailID)) return _playerStates[frame][tailID];
        }
        return null;
    }


    // ------------ PUBLIC METHODS FOR TIME CONN ------------

    // Adds the given player to the Reality Manager, allowing them to time travel.
    public void Connect(int playerID, bool isMe)
	{
		_realities.AddHead(playerID);
		if (isMe) _myID = playerID;
	}

    // Records the given state in all realities this player exists in.
	public virtual void RecordState(PlayerState ps)
	{
		if (TimeEnded()) return;

		int lastTailID = _realities.GetLastTailID(ps.PlayerID);
		List<int> frames = _realities.GetWriteFrames(ps.PlayerID);
		for (int i=0; i < frames.Count; i++)
		{
			ps.TailID = lastTailID + i;
			int frame = frames[i];
            if (_playerStates[frame] == null) _playerStates[frame] = new Dictionary<int, PlayerState>();
			if (_playerStates[frame].ContainsKey(ps.TailID)) _playerStates[frame][ps.TailID] = ps;
			else _playerStates[frame].Add(ps.TailID, ps);
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
		try
		{
			_realities.RemoveWriter(playerID);
		}
		catch (InvalidOperationException e)
		{
			Debug.LogError($"{e}");
		}
	}

	// Finds the closest reality within range of the given player.
	// If there are none, return the player's perceived frame.
	public int GetNearestReality(int playerID)
	{
		int frame = _realities.GetPerceivedFrame(playerID);
		int closestFrame = _realities.GetClosestFrame(playerID);
		if (Math.Abs(closestFrame - frame) < Constants.MinTimeSnapDistance)
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
		return frame;
	}

	// Snaps your position to the nearest reality within range, else creates a new reality.
    // Starts recording in this new reality.
	public void EnterReality(int playerID)
	{
		// Snap to the closest frame.
		int frame = GetNearestReality(playerID);

        // Set your perceived frame and start recording in the new reality.
        SetPerceivedFrame(playerID, frame);
		try
		{
			_realities.AddWriter(playerID, frame);
		}
		catch (InvalidOperationException e)
		{
			Debug.LogError($"{e}");
		}
	}

	// Set the perceived frame of the given player.
	public void SetPerceivedFrame(int playerID, int frame)
	{
		if (frame < 0) frame = 0;
		else if (frame >= _totalFrames) frame = _totalFrames - 1;
		_realities.SetPerceivedFrame(playerID, frame);
	}

	public bool InYourReality(int playerID)
	{
		return _realities.InSameFrame(playerID, _myID);
	}

    // Returns true if the given player can travel in the given direction.
	public bool CanJump(int playerID, Constants.JumpDirection direction)
	{
		if (_currentFrame >= _totalFrames - 1) return false;
		
		int frame = _realities.GetPerceivedFrame(playerID);
		if (direction == Constants.JumpDirection.Backward)
		{
			return (frame - Constants.TimeTravelVelocity) >= 0;
		}
		else
		{
			return (frame + Constants.TimeTravelVelocity) <= _currentFrame;
		}
	}

	public HashSet<int> GetPlayersInReality()
	{
		int frame = _realities.GetPerceivedFrame(_myID);
		return _realities.GetHeadsInFrame(frame);
	}

	public Dictionary<int, Reality> GetRealities() { return _realities.GetRealities(); }

	public void SetRealities(Dictionary<int, Reality> realities) { _realities.SetRealities(realities); }


	// ------------ PUBLIC METHODS ------------

	public int GetCurrentFrame() { return _currentFrame; }

	public int GetTotalFrames() { return _totalFrames; }

	public int GetMyPercievedFrame() { return _realities.GetPerceivedFrame(_myID); }

	public List<(int id, int frame)> GetPerceivedFrames() { return _realities.GetPerceivedFrames(); }

	public int GetYourFrame() { return _realities.GetPerceivedFrame(_myID); }
}
