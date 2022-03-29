using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Reality
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

            public void EndProxy() { UnityEngine.Object.Destroy(Copy); }
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
