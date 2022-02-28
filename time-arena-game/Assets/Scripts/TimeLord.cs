using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class TimeLord : MonoBehaviour
{
    private class TimeTick
    {
        public Vector3 p;
        public Quaternion r;
        private BitArray f;

        public TimeTick()
        {
            f = new BitArray(16, false);
        }

        public bool flags(int i)
        {
            return f[i];
        }

        public void flags(int i, bool b)
        {
            f[i] = b;
        }
    }

    private class TimeStream
    {
        GameObject timeObject;
        int firstTick;
        FractureList<TimeTick> ticks;

        public TimeStream(GameObject t, int start, int length)
        {
            ticks = new FractureList<TimeTick>(length);
            timeObject = t;
            firstTick = start;
        }

        public bool Record(int t)
        {
            //if(t == ticks.getHead())
            //{
               // Debug.Log("Tick Written: " + t.ToString());
                TimeTick newTick = new TimeTick();
                newTick.p = timeObject.transform.position;
                newTick.r = timeObject.transform.rotation;
                newTick.flags(0, true);
                ticks.AddData(newTick);
                return true;
            //}
            //return false;
        }

        public TimeTick[] Recall(int t)
        {
            Debug.Log("Current written ticks: " + ticks.GetHead());
            return ticks.Recall(t);
        }

        public bool checkObjectReference(GameObject t)
        {
            return GameObject.ReferenceEquals(timeObject, t);
        }

        public GameObject GetObject()
        {
            return timeObject;
        }

        public void Travel(int start, int end)
        {
            ticks.AddFracture(start, end);
        }
    }

    private class Reality
    {
        private class Proxy
        {
            readonly TimeStream original;
            public GameObject copy;

            public Proxy(TimeStream t, int id, GameObject copier)
            {
                original = t;
                copy = Object.Instantiate(copier);
                copy.layer = 30 - id;
            }

            public void Update(TimeTick t)
            {
                Debug.Log(t.p.ToString());
                copy.transform.position = t.p;
                copy.transform.rotation = t.r;
            }

            public void EndProxy()
            {
                Destroy(copy);
            }
        }

        List<(TimeStream, List<Proxy>)> observations = new List<(TimeStream, List<Proxy>)>();
        int id = 0;
        public int timeOffset = 0;
        TimeStream owner;
        GameObject copier;
        TimeLord tl;

        public Reality(TimeLord controller, List<TimeStream> streams, int id, TimeStream owner, GameObject copier)
        {
            tl = controller;
            this.id = id;
            this.owner = owner;
            this.copier = copier;
            foreach(TimeStream s in streams)
            {
                observations.Add((s, new List<Proxy>()));
            }
        }

        public void addStream(TimeStream stream)
        {
            observations.Add((stream, new List<Proxy>()));
        }

        public void Advance(int x)
        {
            owner.Travel(tl.GetCurrentTick() + timeOffset, tl.GetCurrentTick() + timeOffset + x);
            timeOffset += x;
            if (timeOffset > 0) timeOffset = 0;

        }

        public void Test()
        {
            Debug.Log("Number of observed objects: " + observations.Count.ToString());
            Debug.Log("Current time offset: " + timeOffset.ToString());
            Debug.Log("Active proxy objects: " + observations[0].Item2.Count.ToString());
        }

        private Proxy CreateProxy(TimeStream t)
        {
            return new Proxy(t, id, copier);
        }

        public void UpdateProxies(int time)
        {
            Test();
            foreach((TimeStream, List<Proxy>) obv in observations)
            {
                Debug.Log("Reality Tick: " + (time + timeOffset).ToString());
                TimeTick[] ticks = obv.Item1.Recall(time + timeOffset);
                if (obv.Item1.checkObjectReference(owner.GetObject()))
                {
                    ticks = ticks.Take(ticks.Length - 1).ToArray();
                }
                // Debug.Log("Tick Count: " + ticks.Length.ToString());
                while(obv.Item2.Count < ticks.Length)
                {
                    obv.Item2.Add(CreateProxy(obv.Item1));
                }
                while(obv.Item2.Count > ticks.Length)
                {
                    obv.Item2[obv.Item2.Count - 1].EndProxy();
                    obv.Item2.RemoveAt(obv.Item2.Count - 1);
                }
                for(int i = 0; i < ticks.Length; i++)
                {
                    obv.Item2[i].Update(ticks[i]);
                }
            }
        }
    }

    [Tooltip("Max number of ticks that can be recorded.")]
    public int maxTicks;
    [Tooltip("Number of ticks per second.")]
    public int tps;
    public GameObject replayer;
    private List<TimeStream> streams = new List<TimeStream>();
    private List<Reality> realities = new List<Reality>();
    private int currentTick = 0;
    public bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        //Set framerate to desired tickrate
        Application.targetFrameRate = tps;
        Debug.Log(FractureList<int>.Test());
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (currentTick >= maxTicks) { active = false; }
            currentTick++;
        }
    }

    private TimeStream FindStream(GameObject t)
    {
        foreach (TimeStream stream in streams)
        {
            if (stream.checkObjectReference(t)) return stream;
        }
        return null;
    }

    public int GetCurrentTick()
    {
        return currentTick;
    }

    public int GetRealityTick(int id){
        return currentTick + realities[id].timeOffset;
    }

    public void AddTimeObject(GameObject t)
    {
        TimeStream newStream = new TimeStream(t, currentTick, maxTicks - currentTick);
        streams.Add(newStream);
    }

    public int AllocateReality(GameObject t)
    {
        Reality newReality = new Reality(this, streams, realities.Count, FindStream(t), replayer);
        realities.Add(newReality);
        return realities.Count - 1;
    }

    public void DestroyReality(int id)
    {
        realities.RemoveAt(id);
    }

    public void Travel(int distance, int id)
    {
        realities[id].Advance(distance);
        realities[id].UpdateProxies(currentTick);
    }

    public void UpdateReality(int id)
    {
        realities[id].UpdateProxies(currentTick);
    }

    public void RealityTest(int id)
    {
        realities[id].Test();
    }

    public void Record(GameObject t)
    {
        TimeStream s = FindStream(t);
        s.Record(currentTick);
    }

    public (Vector3, Quaternion) GetTimeTick(GameObject t, int tick, int age = 0)
    {
        TimeTick[] ticks = FindStream(t).Recall(tick);
        return (ticks[ticks.Length - age].p, ticks[ticks.Length - age].r);
    }

    private void Record()
    {
        foreach(TimeStream s in streams)
        {
            s.Record(currentTick);
        }
    }
}
