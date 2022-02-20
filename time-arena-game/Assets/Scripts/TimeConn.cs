using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeConn : MonoBehaviour
{
    private TimeLord tl;
    private int tick = 0;
    private int lastTick = 0;
    private int timeID = -1;
    // Start is called before the first frame update
    void Start()
    {
        tl = GameObject.FindGameObjectWithTag("TimeLord").GetComponent<TimeLord>();
        tl.AddTimeObject(this.gameObject);
        timeID = tl.AllocateReality(this.gameObject);
        SetCameraLayers();
    }

    // Update is called once per frame
    void Update()
    {
        Tick(tl.GetCurrentTick());
    }

    public void Tick(int t)
    {
        tick = t;
        if(tick != lastTick || tick == 0)
        {
            tl.Record(this.gameObject);
            tl.UpdateReality(timeID);
        }
        lastTick = tick;
    }

    public void TimeJump(int distance)
    {
        Debug.Log("Time Travel Triggered on ID " + timeID.ToString());
        tl.Travel(distance, timeID);
        tl.RealityTest(timeID);
    }

    public float GetTimePosition()
    {
        return (float)tl.GetCurrentTick() / (float)tl.maxTicks;
    }

    private void SetCameraLayers()
    {
        /*
         * 
         * CODE HERE TO SET CAMERA TO ONLY SEE LAYERS > 20
         * IF THEY ARE 30 - timeID
         * 
         */
    }
}
