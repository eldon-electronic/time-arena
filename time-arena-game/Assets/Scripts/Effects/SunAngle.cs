using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//assaign to a directional light
public class SunAngle : MonoBehaviour
{
    TimeLord _timeLord;
    [SerializeField] private AnimationCurve _sunPath;
    [SerializeField] private float _sunMin = 15;
    [SerializeField] private float _sunMax = 80;
    private float _sunDif;

    void Start()
    {   
        if (SceneManager.GetActiveScene().name == "PreGameScene")
        {
            _timeLord = FindObjectOfType<PreGameController>().GetComponent<PreGameController>().GetTimeLord();
        }
        else _timeLord = FindObjectOfType<GameController>().GetComponent<GameController>().GetTimeLord();
        _sunDif = _sunMax - _sunMin;
    }

    // Update is called once per frame
    void Update()
    {
        try {
            //get the percieved frame and total frames
            float percievedFrame = (float) _timeLord?.GetYourFrame();
            float totalFrames = (float) _timeLord?.GetTotalFrames();
            //calculate the percentage frame (P/T)
            float frameFraction = percievedFrame / totalFrames;
            //use lookup in animation curve
            float angleFraction = _sunPath.Evaluate(frameFraction);
            //set light angle to output from curve
            float sunAngle = _sunMax - (_sunDif * angleFraction);
            Vector3 eulerRotation = gameObject.transform.rotation.eulerAngles;
            gameObject.transform.rotation = Quaternion.Euler(
                sunAngle, 
                eulerRotation.y, 
                eulerRotation.z);
        } 
        catch (Exception e) {}
    }
}
