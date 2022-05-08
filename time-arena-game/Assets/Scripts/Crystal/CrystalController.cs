using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class CrystalController : MonoBehaviour
{
    private CrystalData _crystalData; 
    private TimeLord _timeLord;
    private int _startFrame;
    private int _animationLength;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Vector3 _endPosition;
    private Quaternion _endRotation;


    void Awake()
    {
        //get the number of the frame that the animation should start at from the parent prefab. 
        _startFrame = transform.parent.gameObject.GetComponent<CrystalParent>().startFrame;
        string name = gameObject.name + "data";
        _crystalData = GameObject.Find(name).GetComponent<CrystalData>();
        _startPosition = _crystalData.positions[0].Item1;
        _startRotation = _crystalData.positions[0].Item2;
        _endPosition = _crystalData.positions[_crystalData.positions.Count - 1].Item1;
        _endRotation = _crystalData.positions[_crystalData.positions.Count - 1].Item2;
        _animationLength = _crystalData.positions.Count;
    }

    void OnEnable()
    {
        GameController.newTimeLord += OnNewTimeLord;
    }
    
    void OnDisable()
    {
        GameController.newTimeLord -= OnNewTimeLord;
    }

    void Start()
    {
        _timeLord = FindObjectOfType<SceneController>().GetTimeLord();
    }

    private void OnNewTimeLord(TimeLord time)
    {
        _timeLord = time;
    } 

    void Update()
    {
        int frame = _timeLord.GetYourPerceivedFrame();
        // If the current frame being rendered is before the start of the 
        // - animation, then set the local position to default.
        if (frame <= _startFrame)
        {
            transform.localPosition = _startPosition;
            transform.rotation = _startRotation;
        }
        // Else if your current frame is after the animation has ended, then 
        // - set local postion of shard to the final position of the animation. 
        else if (frame >= (_startFrame + _animationLength))
        {
            transform.localPosition = _endPosition;
            transform.rotation = _endRotation;
        }
        else
        {
            transform.localPosition = _crystalData.positions[frame - _startFrame].Item1;
            transform.rotation = _crystalData.positions[frame - _startFrame].Item2;
        }
    }

}
