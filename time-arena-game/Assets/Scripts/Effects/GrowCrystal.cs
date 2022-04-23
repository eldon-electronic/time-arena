using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//assaign to a directional light
public class GrowCrystal : MonoBehaviour
{
    TimeLord _timeLord;
    [SerializeField] private AnimationCurve _growthPath;
    [SerializeField] private float _startFrame = 1000;
    [SerializeField] private float _growthTime = 90;
    [SerializeField] private BoxCollider _ObjectCollider;
    private float _endFrame;
    private float _ScaleBase = 1;
    private float _yMax = 45;
    private float _xzScale = 45;
    private float _yPosStart;
    private Vector3 _startPos;


    void Start()
    {   
        if (SceneManager.GetActiveScene().name == "PreGameScene")
        {
            _timeLord = FindObjectOfType<PreGameController>().GetComponent<PreGameController>().GetTimeLord();
        }
        else _timeLord = FindObjectOfType<GameController>().GetComponent<GameController>().GetTimeLord();
       _endFrame = _startFrame + _growthTime;
       _startPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        try {
            //get the percieved frame
            float percievedFrame = (float) _timeLord?.GetYourFrame();

            if( (percievedFrame >= _startFrame) && (percievedFrame <= _endFrame) )
            {
                //get progress modifier from curve
                float progress = (_endFrame - percievedFrame) / _growthTime;
                float gP = _growthPath.Evaluate(progress);
                //modifiy values by progress
                float xzSChange = (gP*_xzScale) + _ScaleBase;
                float ySChange = (gP*_yMax) + _ScaleBase;
                float yPosChange = (gP* 2.15f);
                Vector3 scaleChange = new Vector3(xzSChange,ySChange,xzSChange);
                Vector3 position = gameObject.transform.position;
                Vector3 positionChange = _startPos;
                positionChange[1] +=  yPosChange;
                gameObject.transform.position = positionChange;
                gameObject.transform.localScale = scaleChange;
            } 
            else if (percievedFrame < _startFrame)
            {
                _ObjectCollider.isTrigger = true;
            } 
            else if (percievedFrame > _startFrame) 
            {
                _ObjectCollider.isTrigger = false;
            }
        } 
        catch (Exception e) {}
    }
}
