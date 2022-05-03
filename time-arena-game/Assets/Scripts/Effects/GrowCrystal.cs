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
    private float _yPosChange = 2.15f;
    private Vector3 _startPos;


    void Start()
    {   
        _timeLord = FindObjectOfType<SceneController>().GetTimeLord();
       _endFrame = _startFrame + _growthTime;
       _startPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        try {
            //get the percieved frame
            float percievedFrame = (float) _timeLord?.GetYourPerceivedFrame();
            Vector3 scaleChange = new Vector3(1f, 1f, 1f);
            Vector3 position = gameObject.transform.position;
            Vector3 positionChange = _startPos;

            if( (percievedFrame >= _startFrame) && (percievedFrame <= _endFrame) )
            {
                //get progress modifier from curve
                float progress = (_endFrame - percievedFrame) / _growthTime;
                float gP = _growthPath.Evaluate(progress);
                //modifiy values by progress
                float xzSChange = (gP*_xzScale) + _ScaleBase;
                float ySChange = (gP*_yMax) + _ScaleBase;
                float yPosChange = (gP* _yPosChange);
                scaleChange = new Vector3(xzSChange,ySChange,xzSChange);
                positionChange = _startPos;
                positionChange[1] +=  yPosChange;
            } 
            else if (percievedFrame < _startFrame)
            {
                //turn collider off
                _ObjectCollider.isTrigger = true;
                //set origonal scale and pos
                scaleChange = new Vector3(_ScaleBase,_ScaleBase,_ScaleBase);
                positionChange = _startPos;
            } 
            else if (percievedFrame > _startFrame) 
            {
                //turn collider off
                _ObjectCollider.isTrigger = false;
                //set post growth scale and position
                scaleChange = new Vector3(_xzScale,_yMax,_xzScale);
                positionChange = _startPos;
                positionChange[1] += _yPosChange;
            }
            
            gameObject.transform.position = positionChange;
            gameObject.transform.localScale = scaleChange;
        } 
        catch (Exception e) {Debug.LogError($"Error: {e}");}
    }
}
