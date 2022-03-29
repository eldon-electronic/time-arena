using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Reality
    {
        public int PerceivedFrame;
        public List<int> WriteFrames;
        public int LastTailID;
        public int Countdown;

        public Reality()
        {
            PerceivedFrame = 0;
            WriteFrames = new List<int>();
            LastTailID = 0;
            Countdown = -1;
        }

        public void Increment()
        {
            PerceivedFrame++;
            for (int i=0; i < WriteFrames.Count; i++)
            {
                WriteFrames[i]++;
            }
            if (Countdown > -1) Countdown--;
        }
    }


public class RealityManager
{
    private Dictionary<int, Reality> _realities;

    public RealityManager()
    {
        _realities = new Dictionary<int, Reality>();
    }

    // Add a new player to the dictionary.
    public void AddHead(int playerID)
    {
        _realities.Add(playerID, new Reality());
    }

    // Increment the frame values of every player.
    // Remove a reality's writer if its countdown reaches 0.
    public void Tick()
    {
        foreach (var reality in _realities)
        {
            reality.Value.Increment();
            if (reality.Value.Countdown == 0)
            {
                reality.Value.WriteFrames.RemoveAt(0);
                reality.Value.LastTailID++;
            }
        }
    }

    // Return the perceived frame of the given player.
    public int GetPerceivedFrame(int playerID)
    {
        return _realities[playerID].PerceivedFrame;
    }

    // Return the player ID and perceived frame of all players in the dictionary.
    public List<(int id, int frame)> GetPerceivedFrames()
    {
        List<(int, int)> frameData = new List<(int, int)>();
        foreach (var reality in _realities)
        {
            frameData.Add((reality.Key, reality.Value.PerceivedFrame));
        }
        return frameData;
    }

    // Add the given offset to the given player's perceived frame.
    public void OffsetPerceivedFrame(int playerID, int offset)
    {
        _realities[playerID].PerceivedFrame += offset;
    }

    // Return the write frames of the given player.
    // Each player may have 0, 1 or 2 write frames at any time.
    public List<int> GetWriteFrames(int playerID)
    {
        return _realities[playerID].WriteFrames;
    }

    // Make the given player start writing their state to a new place, starting from the given frame.
    public void AddWriter(int playerID, int frame)
    {
        if (_realities[playerID].WriteFrames.Count == 2)
        {
            Debug.LogError("Cannot write to more than two frames simultaneously.");
        }
        else _realities[playerID].WriteFrames.Add(frame);
    }

    // Remove the earliest writer from the given player.
    // Starts a countdown to remove the writer after enough frames have passed to play a dissolve animation.
    public void RemoveWriter(int playerID)
    {
        if (_realities[playerID].WriteFrames.Count == 0)
        {
            Debug.LogError("No tracked writers to remove.");
        }
        else
        {
            _realities[playerID].Countdown = Constants.AnimationFrames;
        }
    }

    // Given a player and their frame, return the closest frame of a different player.
    // This should be from the last reality they were writing to.
    public int GetClosestFrame(int playerID, int frame)
    {
        int closest = int.MaxValue;
        foreach(var reality in _realities)
        {
            if (reality.Key != playerID)
            {
                if (reality.Value.WriteFrames.Count > 0)
                {
                    int f = reality.Value.WriteFrames.Last();
                    if (Mathf.Abs(f - frame) < Mathf.Abs(closest - frame))
                    {
                        closest = f;
                    }
                }
            }
        }
        return closest;
    }

    // TODO: Use this function for finding out which heads (actual players) you need to render.
    // Return a list of player IDs for those players who exist in the same reality
    // as the given frame.
    public List<int> GetHeadsInFrame(int frame)
    {
        List<int> heads = new List<int>();
        foreach (var reality in _realities)
        {
            foreach (var f in reality.Value.WriteFrames)
            {
                if (f == frame) heads.Add(reality.Key);
            }
        }
        return heads;
    }

    // Return the tail ID of the last reality the given player was writing to.
    public int GetLastTailID(int playerID) { return _realities[playerID].LastTailID; }

    // Return the tail ID of the current reality the given player is writing to.
    public int GetNextTailID(int playerID)
    {
        if (_realities[playerID].WriteFrames.Count <= 1) return _realities[playerID].LastTailID;
        else return _realities[playerID].LastTailID + 1;
    }


    // WARNING: The following function is to be used by test framework only.
    public Dictionary<int, Reality> RevealHeads() { return _realities; }
}
