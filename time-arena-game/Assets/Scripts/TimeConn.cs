using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TimeConn : MonoBehaviour
{
    private TimeLord _tl;
    private int _tick;
    private int _lastTick;
    private int _timeID;
    
    void Start()
    {
      _tick = 0;
      _lastTick = 0;
      _timeID = -1;
    }

    public void ConnectToTimeLord()
    {
      _tl = GameObject.FindGameObjectWithTag("TimeLord").GetComponent<TimeLord>();
      _tl.AddTimeObject(this.gameObject);
      _timeID = _tl.AllocateReality(this.gameObject);
      SetCameraLayers();
    }

    void Update()
    {
      if (SceneManager.GetActiveScene().name == "GameScene")
      {
        if (_tl == null) ConnectToTimeLord();
        Tick(_tl.GetCurrentTick());
      }
    }

    public void Tick(int t)
    {
        _tick = t;
        if (_tick != _lastTick || _tick == 0)
        {
            _tl.Record(this.gameObject);
            _tl.UpdateReality(_timeID);
        }
        _lastTick = _tick;
    }

    public void TimeJump(int distance)
    {
        // Debug.Log("Time Travel Triggered on ID " + timeID.ToString());
        _tl.Travel(distance, _timeID);
        _tl.RealityTest(_timeID);
    }

    public float GetTimePosition()
    {
        return (float) _tl.GetRealityTick(_timeID) / (float) _tl.maxTicks;
    }

    public float GetRealityTick() 
    {
      return (float) _tl.GetRealityTick(_timeID);
    }

    public float GetCurrentTick() 
    {
      return (float) _tl.GetCurrentTick();
    }

    public float MaxTick() 
    {
      return (float) _tl.maxTicks;
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
