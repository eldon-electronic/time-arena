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
}




















public class OldReality
    {
        private class Proxy
        {
            readonly TimeStream Original;
            public GameObject Copy;

            public Proxy(TimeStream t, int id, GameObject copier)
            {
                Original = t;
                Copy = UnityEngine.Object.Instantiate(copier);
                Copy.layer = 30 - id;
            }

            public void Update(TimeTick t)
            {
                // Debug.Log(t.p.ToString());
                Copy.transform.position = t.Pos;
                Copy.transform.rotation = t.Rot;
            }

            public void EndProxy() { UnityEngine.Object.Destroy(Copy); }
        }

        private List<(TimeStream, List<Proxy>)> _observations = new List<(TimeStream, List<Proxy>)>();
        private int _id = 0;
        public int TimeOffset = 0;
        private TimeStream _owner;
        private GameObject _copier;
        private TimeLord _tl;

        public OldReality(TimeLord controller, List<TimeStream> streams, int id, TimeStream owner, GameObject copier)
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
