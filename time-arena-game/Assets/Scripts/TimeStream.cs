using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeTick
{
    public Vector3 Pos;
    public Quaternion Rot;
    private BitArray _flags;

    public TimeTick() { _flags = new BitArray(16, false); }

    public bool GetFlag(int i) { return _flags[i]; }

    public void SetFlag(int i, bool b) { _flags[i] = b; }
}


public class TimeStream
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
