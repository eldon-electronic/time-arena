using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TimeLord : MonoBehaviour
{
    private class TimeStream
    {
        GameObject timeObject;
        int firstTick;
        bool[] recordedTicks;
        Vector3[] positions;
        Quaternion[] rotations;

        public void Init(GameObject t, int start, int length)
        {
            timeObject = t;
            firstTick = start;
            recordedTicks = new bool[length];
            positions = new Vector3[length];
            rotations = new Quaternion[length];
        }

        public bool Record(int t, Vector3 v, Quaternion r)
        {
            try
            {
                positions[t] = v;
                rotations[t] = r;
                recordedTicks[t] = true;
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
                positions[t] = timeObject.transform.position;
                rotations[t] = timeObject.transform.rotation;
                recordedTicks[t] = true;
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
                if (!recordedTicks[t])
                {
                    int a = predec(t);
                    int b = postdec(t);
                    float p = 0;
                    if(a == -1 || b == -1) { throw new System.Exception("no valid recorded ticks found"); }
                    if (a != b){ p = (float)(t - a) / (float)(b - a); }
                    return (Vector3.Lerp(positions[a], positions[b], p), Quaternion.Lerp(rotations[a], rotations[b], p));
                }
                return (positions[t], rotations[t]);
            }
            catch
            {
                return (Vector3.zero, Quaternion.identity);
            }
        }

        // find last save tick
        private int predec(int t)
        {
            int i = t;
            do
            {
                if (recordedTicks[i]) return i;
                i--;
            } while (i >= 0);
            return -1;
        }
        
        // find next saved tick
        private int postdec(int t)
        {
            int i = t;
            int k = recordedTicks.Length;
            do
            {
                if (recordedTicks[i]) return i;
                i++;
            } while (i < k);
            return -1;
        }
    }

    public GameObject player1;
    public int maxgameTicks;
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
        StartCoroutine(TimeClock());
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
