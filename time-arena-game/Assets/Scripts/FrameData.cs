using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FrameData
{
    private int _perceivedFrame;
    private List<int> _tailFrames;
    private int _lastTailID;

    public FrameData()
    {
        _perceivedFrame = 0;
        _tailFrames = new List<int>();
        _tailFrames.Add(0);
        _lastTailID = 0;
    }

    // Returns this player's perceived frame.
    public int GetPerceivedFrame() { return _perceivedFrame; }

    // Adds the given offset to the perceived frame.
    public void OffsetPerceivedFrame(int offset) { _perceivedFrame += offset; }

    // Returns this player's tail frames (the pointers to where we're writing to in the player states structure).
    // There may be 0, 1 or 2 tail frames at any one time.
    public List<int> GetTailFrames() { return _tailFrames; }

    // Returns the tail frame pointer of the latest reality we're writing to.
    public int GetLatestFrame() { return _tailFrames.Last(); }

    // Increments every frame (perceived frame and tail frames).
    public void Increment()
    {
        _perceivedFrame++;
        for (int i=0; i < _tailFrames.Count; i++)
        {
            _tailFrames[i]++;
        }
    }

    // Returns the tail ID of the last reality we were writing to.
    public int GetLastTailID() { return _lastTailID; }

    // Returns the tail ID of the next reality we are writing to.
    public int GetNextTailID()
    {
        if (_tailFrames.Count <= 1) return _lastTailID;
        else return _lastTailID + 1;
    }

    // Start writing to a new reality at the given frame. i.e. add a tail writer pointer.
    public void AddTail(int frame)
    {
        if (_tailFrames.Count == 2) Debug.LogError("Cannot write to more than two tails simultaneously.");
        else _tailFrames.Add(frame);
    }

    // Stop writing to the last reality you were at. i.e. remove the first tail writer pointer.
    public void RemoveTail()
    {
        if (_tailFrames.Count == 0) Debug.LogError("No tracked tails to remove.");
        else
        {
            _tailFrames.RemoveAt(0);
            _lastTailID++;
        }
    }
}
