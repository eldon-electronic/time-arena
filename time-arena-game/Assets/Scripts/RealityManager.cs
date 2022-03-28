using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RealityManager
{
    private Dictionary<int, FrameData> _heads;

    public RealityManager()
    {
        _heads = new Dictionary<int, FrameData>();
    }

    // Add a new player to the dictionary.
    public void AddHead(int playerID)
    {
        _heads.Add(playerID, new FrameData());
    }

    // Increment the frame values of every player.
    public void Increment()
    {
        foreach (var head in _heads)
        {
            head.Value.Increment();
        }
    }

    // Return the perceived frame of the given player.
    public int GetPerceivedFrame(int playerID)
    {
        FrameData frames = _heads[playerID];
        return frames.GetPerceivedFrame();
    }

    // Return the player ID and perceived frame of all players in the dictionary.
    public List<(int id, int frame)> GetPerceivedFrames()
    {
        List<(int, int)> frames = new List<(int, int)>();
        foreach (var head in _heads)
        {
            frames.Add((head.Key, head.Value.GetPerceivedFrame()));
        }
        return frames;
    }

    // Add the given offset to the given player's perceived frame.
    public void OffsetPerceivedFrame(int playerID, int offset)
    {
        FrameData frames = _heads[playerID];
        frames.OffsetPerceivedFrame(offset);
    }

    // Return the tail writer pointers of the given player.
    // Each player may have 0, 1 or 2 tail frames at any time.
    public List<int> GetTailFrames(int playerID)
    {
        FrameData frames = _heads[playerID];
        return frames.GetTailFrames();
    }

    // Make the given player start writing their state to a new reality,
    // starting from the given frame.
    // i.e. add a new tail writer pointer with the given frame.
    public void AddTail(int playerID, int frame)
    {
        _heads[playerID].AddTail(frame);
    }

    // Remove the earliest tail writer pointer from the given player.
    public void RemoveTail(int playerID)
    {
        _heads[playerID].RemoveTail();
    }

    // Given a player and their frame, return the closest frame of a different player.
    // This should be from the last reality they were writing to.
    public int GetClosestFrame(int playerID, int frame)
    {
        int closest = int.MaxValue;
        foreach(var head in _heads)
        {
            if (head.Key != playerID)
            {
                int f = head.Value.GetLatestFrame();
                if (Mathf.Abs(f - frame) < Mathf.Abs(closest - frame))
                {
                    closest = f;
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
        foreach (var head in _heads)
        {
            foreach (var f in head.Value.GetTailFrames())
            {
                if (f == frame) heads.Add(head.Key);
            }
        }
        return heads;
    }

    // Return the tail ID of the last reality the given player was writing to.
    public int GetLastTailID(int playerID) { return _heads[playerID].GetLastTailID(); }

    // Return the tail ID of the current reality the given player is writing to.
    public int GetNextTailID(int playerID) { return _heads[playerID].GetNextTailID(); }


    // WARNING: The following function is to be used by test framework only.
    public Dictionary<int, FrameData> RevealHeads() { return _heads; }
}
