using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TimeConn : MonoBehaviour
{
    private TimeLord tl;
    private int tick = 0;
    private int lastTick = 0;
    private int timeID = -1;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void connectToTimeLord(){
      tl = GameObject.FindGameObjectWithTag("TimeLord").GetComponent<TimeLord>();
      tl.AddTimeObject(this.gameObject);
      timeID = tl.AllocateReality(this.gameObject);
      SetCameraLayers();
    }

    // Update is called once per frame
    void Update()
    {
      if(SceneManager.GetActiveScene().name == "GameScene"){
        if(tl == null){ connectToTimeLord(); }
        Tick(tl.GetCurrentTick());
      }
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
        return (float)tl.GetRealityTick(timeID) / (float)tl.maxTicks;
    }

    public float GetRealityTick() 
    {
      return (float) tl.GetRealityTick(timeID);
    }

    public float GetCurrentTick() 
    {
      return (float) tl.GetCurrentTick();
    }

    public float MaxTick() 
    {
      return (float) tl.maxTicks;
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
