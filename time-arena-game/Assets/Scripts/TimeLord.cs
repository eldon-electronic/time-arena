using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TimeLord : MonoBehaviour
{

    private class TimeTick
    {
        public Vector3 p;
        public Quaternion r;
        public BitArray flags;

        /*-----------------------------------------
        flags:
             0 - has tick been recorded?
             1 - jump key pressed?
             2 - forward key pressed?
             3 - backward key pressed?
             4 - left key pressed?
             5 - right key pressed?
             6 - lmb pressed?
             7 - rmb pressed?
             8 - mmb pressed?
             9 - crouch key pressed?
            10 - available
            11 - available
            12 - available
            13 - available
            14 - available
            15 - available
        -----------------------------------------*/

        public TimeTick()
        {
            flags = new BitArray(16, false);
        }
    }

    private class TimeStream
    {
        GameObject timeObject;
        int firstTick;
        TimeTick[] ticks;
        int compressionFactor = 1;

        public void Init(GameObject t, int start, int length)
        {
            ticks = new TimeTick[length];
            timeObject = t;
            firstTick = start;
        }

        public bool Record(int t, Vector3 v, Quaternion r)
        {
            try
            {
                ticks[t] = new TimeTick();
                ticks[t].p = v;
                ticks[t].r = r;
                ticks[t].flags[0] = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Record(int t)
        {
            try
            {
                ticks[t] = new TimeTick();
                ticks[t].p = timeObject.transform.position;
                ticks[t].r = timeObject.transform.rotation;
                ticks[t].flags[0] = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public (Vector3, Quaternion) Recall(int t)
        {
            try
            {
                if (ticks[t] == null)
                {
                    int a = predec(t);
                    int b = postdec(t);
                    float p = 0;
                    if (a == -1 || b == -1) { Debug.Log("no valid recorded ticks found"); throw new System.Exception(); }
                    if (a != b){ p = (float)(t - a) / (float)(b - a); }
                    Debug.Log("t: " + t.ToString() + " a: " + a.ToString() + " b: " + b.ToString() + " p: " + p.ToString());
                    return (Vector3.Lerp(ticks[a].p, ticks[b].p, p), Quaternion.Lerp(ticks[a].r, ticks[b].r, p));
                }
                return (ticks[t].p, ticks[t].r);
            }
            catch
            {
                return (Vector3.zero, Quaternion.identity);
            }
        }

        public bool checkObjectReference(GameObject t)
        {
            return GameObject.ReferenceEquals(timeObject, t);
        }

        // find last save tick
        private int predec(int t)
        {
            int i = t;
            do
            {
                if (ticks[i] != null) return i;
                i--;
            } while (i >= firstTick);
            return -1;
        }

        // find next saved tick
        private int postdec(int t)
        {
            int i = t;
            int k = ticks.Length;
            do
            {
                if (ticks[i] != null) return i;
                i++;
            } while (i < k);
            return -1;
        }

        public int compression(){
            return compressionFactor;
        }
    }

    public GameObject[] trackables;
    public int maxgameTicks;
    public bool active = false;
    private bool recording = true;
    private bool replaying;
    private List<TimeStream> streams = new List<TimeStream>();
    private int currentTick = 0;
    private GameObject replay;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject o in trackables){
            TimeStream t = new TimeStream();
            t.Init(o, 0, maxgameTicks);
            streams.Add(t);
        }
    }

    private void Update()
    {
        if(currentTick >= maxgameTicks){ recording = false; replaying = false; currentTick = 0;}
        if (currentTick > 30 && !replaying)
        {
            replay = GameObject.CreatePrimitive(PrimitiveType.Cube);
            replaying = true;
        }
        if(recording){ Record(); }
        if(replaying){ Replay(replay); }
        currentTick++;
    }

    private TimeStream findStream(GameObject o){
        foreach(TimeStream stream in streams){
            if(stream.checkObjectReference(o)) return stream;
        }
        return null;
    }

    public int getCurrentTick(){
        return currentTick;
    }
    public void AddNewTimeObject(GameObject t)
    {
        TimeStream newStream = new TimeStream();
        newStream.Init(t, currentTick, maxgameTicks - currentTick);
        streams.Add(newStream);
    }

    public void timeJump(GameObject t, int distance){
        Debug.Log(t.ToString());
        (Vector3, Quaternion) newTimePoint = getTimeTick(t, currentTick - distance);
        t.transform.position = newTimePoint.Item1;
        t.transform.rotation = newTimePoint.Item2;
    }

    public bool forceRecord(GameObject t)
    {
        foreach(TimeStream stream in streams)
        {
            if (stream.checkObjectReference(t))
            {
                stream.Record(currentTick);
                Debug.Log("found");
                return true;
            }
        }
        return false;
    }

    public (Vector3, Quaternion) getTimeTick(GameObject o, int tick){
      Debug.Log(o.ToString());
      return findStream(o).Recall(tick);
    }

    private bool Record(){
        foreach(TimeStream s in streams){
            if(currentTick % s.compression() == 0){
                s.Record(currentTick);
            }
        }
        return true;
    }

    private bool Replay(GameObject replayer){
        (Vector3, Quaternion) data = streams[0].Recall(currentTick - 30);
        replayer.transform.position = data.Item1 + new Vector3(-10, 0, 0);
        replayer.transform.rotation = data.Item2;
        return true;
    }
}
