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
            Debug.Log(ticks[t].flags[0].ToString());
            try
            {
                if (ticks[t].flags[0] == false)
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
                if (ticks[i].flags[0]) return i;
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
                if (ticks[i].flags[0]) return i;
                i++;
            } while (i < k);
            return -1;
        }
    }

    public GameObject player1;
    public int maxgameTicks;
    public bool active = false;
    private bool recording = true;
    private bool replaying;
    private TimeStream stream1;
    private List<TimeStream> streams;
    private int currentTick = 0;
    // Start is called before the first frame update
    void Start()
    {
        stream1 = new TimeStream();
        stream1.Init(player1, 0, maxgameTicks);
        Debug.Log(Marshal.SizeOf(typeof(Vector3)));
        Debug.Log(Marshal.SizeOf(typeof(Quaternion)));
        Debug.Log(Marshal.SizeOf(typeof(bool)));
        if (active)
        {
            StartCoroutine(TimeClock());
        }
    }

    private void Update()
    {
        if (!recording && !replaying)
        {
            GameObject replay = GameObject.CreatePrimitive(PrimitiveType.Cube);
            StartCoroutine(TimeReplay(replay));
        }
    }

    public void AddNewTimeObject(GameObject t)
    {
        TimeStream newStream = new TimeStream();
        newStream.Init(t, currentTick, maxgameTicks - currentTick);
        streams.Add(newStream);
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

    IEnumerator TimeClock()
    {
        recording = true;
        for(int i = 0; i < maxgameTicks; i++)
        {
            if(i % 3 == 0) stream1.Record(i);
            currentTick = i + 1;
            yield return new WaitForSecondsRealtime(0.03125f);
        }
        recording = false;
    }

    IEnumerator TimeReplay(GameObject replayer)
    {
        replaying = true;
        for (int i = 0; i < maxgameTicks; i++)
        {
            (Vector3, Quaternion) data = stream1.Recall(i);
            replayer.transform.position = data.Item1;
            replayer.transform.rotation = data.Item2;
            yield return new WaitForSecondsRealtime(0.03125f);
        }
    }
}
