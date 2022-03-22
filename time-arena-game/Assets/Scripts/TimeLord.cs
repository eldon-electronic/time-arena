using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class TimeLord : MonoBehaviour
{
    [Tooltip("Max number of ticks that can be recorded.")]
    public int maxTicks;
    [Tooltip("Number of ticks per second.")]
    private int _tps;
    public GameObject Replayer;
    private List<TimeStream> _streams = new List<TimeStream>();
    private List<Reality> _realities = new List<Reality>();
    private int _currentTick = 0;
    public bool Active = false;
    

    void Start()
    {
        // Set framerate to desired tickrate.
        Application.targetFrameRate = _tps;
        // Debug.Log(FractureList<int>.Test());
    }

    void Update()
    {
        if (Active)
        {
            if (_currentTick >= maxTicks) Active = false;
            _currentTick++;
        }
    }

    private TimeStream FindStream(GameObject t)
    {
        foreach (TimeStream stream in _streams)
        {
            if (stream.CheckObjectReference(t)) return stream;
        }
        return null;
    }

    public int GetCurrentTick() { return _currentTick; }

    public int GetRealityTick(int id) { return _currentTick + _realities[id].TimeOffset; }

    public void AddTimeObject(GameObject t)
    {
        TimeStream newStream = new TimeStream(t, _currentTick, maxTicks - _currentTick);
        _streams.Add(newStream);
    }

    public int AllocateReality(GameObject t)
    {
        Reality newReality = new Reality(this, _streams, _realities.Count, FindStream(t), Replayer);
        _realities.Add(newReality);
        return _realities.Count - 1;
    }

    public void DestroyReality(int id) { _realities.RemoveAt(id); }

    public void Travel(int distance, int id)
    {
        _realities[id].Advance(distance);
        _realities[id].UpdateProxies(_currentTick);
    }

    public void UpdateReality(int id) { _realities[id].UpdateProxies(_currentTick); }

    public void RealityTest(int id) { _realities[id].Test(); }

    public void Record(GameObject t)
    {
        TimeStream s = FindStream(t);
        s.Record(_currentTick);
    }

    public (Vector3, Quaternion) GetTimeTick(GameObject t, int tick, int age = 0)
    {
        TimeTick[] ticks = FindStream(t).Recall(tick);
        return (ticks[ticks.Length - age].Pos, ticks[ticks.Length - age].Rot);
    }

    private void Record()
    {
        foreach(TimeStream s in _streams)
        {
            s.Record(_currentTick);
        }
    }
}
