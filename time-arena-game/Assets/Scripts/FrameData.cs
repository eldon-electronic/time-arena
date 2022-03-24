using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FrameData
{
    private int _perceivedFrame;
    private List<int> _tailFrames;

    public FrameData()
    {
        _perceivedFrame = 0;
        _tailFrames = new List<int>();
        _tailFrames.Add(0);
    }

    public int GetPerceivedFrame() { return _perceivedFrame; }

    public void OffsetPerceivedFrame(int offset) { _perceivedFrame += offset; }

    public List<int> GetTailFrames() { return _tailFrames; }

    public int GetLatestFrame() { return _tailFrames.Last(); }

    public async void Increment()
    {
        _perceivedFrame++;
        for (int i=0; i < _tailFrames.Count; i++)
        {
            _tailFrames[i]++;
        }
    }

    public void AddTail(int frame)
    {
        if (_tailFrames.Count == 2) Debug.LogError("Cannot write to more than two tails simultaneously.");
        else _tailFrames.Add(frame);
    }

    public void RemoveTail()
    {
        if (_tailFrames.Count == 0) Debug.LogError("No tracked tails to remove.");
        else _tailFrames.RemoveAt(0);
    }
}
