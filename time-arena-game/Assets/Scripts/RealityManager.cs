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

    public int Size() { return _heads.Count; }

    public void AddHead(int playerID)
    {
        _heads.Add(playerID, new FrameData());
    }

    public void Increment()
    {
        foreach (var head in _heads)
        {
            head.Value.Increment();
        }
    }

    public int GetPerceivedFrame(int playerID)
    {
        FrameData frames = _heads[playerID];
        return frames.GetPerceivedFrame();
    }

    public List<(int id, int frame)> GetPerceivedFrames()
    {
        List<(int, int)> frames = new List<(int, int)>();
        foreach (var head in _heads)
        {
            frames.Add((head.Key, head.Value.GetPerceivedFrame()));
        }
        return frames;
    }

    public void OffsetPerceivedFrame(int playerID, int offset)
    {
        FrameData frames = _heads[playerID];
        frames.OffsetPerceivedFrame(offset);
    }

    public List<int> GetTailFrames(int playerID)
    {
        FrameData frames = _heads[playerID];
        return frames.GetTailFrames();
    }

    public void AddTail(int playerID, int frame)
    {
        _heads[playerID].AddTail(frame);
    }

    public void RemoveTail(int playerID)
    {
        _heads[playerID].RemoveTail();
    }

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

    public int GetLastTailID(int playerID) { return _heads[playerID].GetLastTailID(); }
}
