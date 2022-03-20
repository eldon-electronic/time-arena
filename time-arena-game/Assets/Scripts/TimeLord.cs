using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class TimeLord : MonoBehaviour
{
    private class TimeTick
    {
        public Vector3 Pos;
        public Quaternion Rot;
        private BitArray _flags;

        public TimeTick() { _flags = new BitArray(16, false); }

        public bool GetFlag(int i) { return _flags[i]; }

        public void SetFlag(int i, bool b) { _flags[i] = b; }
    }

    private class TimeStream
    {
        private GameObject _timeObject;
        private int _firstTick;
        private FractureList<TimeTick> _ticks;

        public TimeStream(GameObject t, int start, int length)
        {
            _ticks = new FractureList<TimeTick>(length);
            _timeObject = t;
            _firstTick = start;
        }

        public bool Record(int t)
        {
            //if(t == ticks.getHead())
            //{
               // Debug.Log("Tick Written: " + t.ToString());
                TimeTick newTick = new TimeTick();
                newTick.Pos = _timeObject.transform.position;
                newTick.Rot = _timeObject.transform.rotation;
                newTick.SetFlag(0, true);
                _ticks.AddData(newTick);
                return true;
            //}
            //return false;
        }

        public TimeTick[] Recall(int t)
        {
            // Debug.Log("Current written ticks: " + ticks.GetHead());
            return _ticks.Recall(t);
        }

        public bool CheckObjectReference(GameObject t) { return GameObject.ReferenceEquals(_timeObject, t); }

        public GameObject GetObject() { return _timeObject; }

        public void Travel(int start, int end) { _ticks.AddFracture(start, end); }
    }

    private class Reality
    {
        private class Proxy
        {
            readonly TimeStream Original;
            public GameObject Copy;

            public Proxy(TimeStream t, int id, GameObject copier)
            {
                Original = t;
                Copy = Object.Instantiate(copier);
                Copy.layer = 30 - id;
            }

            public void Update(TimeTick t)
            {
                // Debug.Log(t.p.ToString());
                Copy.transform.position = t.Pos;
                Copy.transform.rotation = t.Rot;
            }

            public void EndProxy() { Destroy(Copy); }
        }

        private List<(TimeStream, List<Proxy>)> _observations = new List<(TimeStream, List<Proxy>)>();
        private int _id = 0;
        public int TimeOffset = 0;
        private TimeStream _owner;
        private GameObject _copier;
        private TimeLord _tl;

        public Reality(TimeLord controller, List<TimeStream> streams, int id, TimeStream owner, GameObject copier)
        {
            _tl = controller;
            this._id = id;
            this._owner = owner;
            this._copier = copier;
            foreach(TimeStream s in streams)
            {
                _observations.Add((s, new List<Proxy>()));
            }
        }

        public void AddStream(TimeStream stream)
        {
            _observations.Add((stream, new List<Proxy>()));
        }

        public void Advance(int x)
        {
            _owner.Travel(_tl.GetCurrentTick() + TimeOffset, _tl.GetCurrentTick() + TimeOffset + x);
            TimeOffset += x;
            if (TimeOffset > 0) TimeOffset = 0;

        }

        public void Test()
        {
            // Debug.Log("Number of observed objects: " + observations.Count.ToString());
            // Debug.Log("Current time offset: " + timeOffset.ToString());
            // Debug.Log("Active proxy objects: " + observations[0].Item2.Count.ToString());
        }

        private Proxy CreateProxy(TimeStream t) { return new Proxy(t, _id, _copier); }

        public void UpdateProxies(int time)
        {
            Test();
            foreach((TimeStream, List<Proxy>) obv in _observations)
            {
                // Debug.Log("Reality Tick: " + (time + timeOffset).ToString());
                TimeTick[] ticks = obv.Item1.Recall(time + TimeOffset);
                if (obv.Item1.CheckObjectReference(_owner.GetObject()))
                {
                    ticks = ticks.Take(ticks.Length - 1).ToArray();
                }
                // Debug.Log("Tick Count: " + ticks.Length.ToString());
                while (obv.Item2.Count < ticks.Length)
                {
                    obv.Item2.Add(CreateProxy(obv.Item1));
                }
                while (obv.Item2.Count > ticks.Length)
                {
                    obv.Item2[obv.Item2.Count - 1].EndProxy();
                    obv.Item2.RemoveAt(obv.Item2.Count - 1);
                }
                for (int i = 0; i < ticks.Length; i++)
                {
                    obv.Item2[i].Update(ticks[i]);
                }
            }
        }
    }

    [Tooltip("Max number of ticks that can be recorded.")]
    public int maxTicks;
    [Tooltip("Number of ticks per second.")]
    private int _tps;
    public GameObject replayer;
    private List<TimeStream> _streams = new List<TimeStream>();
    private List<Reality> _realities = new List<Reality>();
    private int _currentTick = 0;
    public bool active = false;
    

    void Start()
    {
        // Set framerate to desired tickrate.
        Application.targetFrameRate = _tps;
        // Debug.Log(FractureList<int>.Test());
    }

    void Update()
    {
        if (active)
        {
            if (_currentTick >= maxTicks) active = false;
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
        Reality newReality = new Reality(this, _streams, _realities.Count, FindStream(t), replayer);
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
