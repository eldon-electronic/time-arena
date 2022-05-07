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
    [SerializeField] private GameObject _nameTag;
    private float _endFrame;
    private float _ScaleBase = 1;
    private float _yMax = 45;
    private float _xzScale = 45;
    private float _yPosStart;
    private float _yPosChange = 2.15f;
    private Transform _playerTransform;
    private TimeConn _playerTime;
    private Vector3 _startPos;
    private bool _activeHints;


    void Awake() { _activeHints = true; }

    void OnEnable() { PlayerController.clientEntered += OnClientEntered; }
    
    void OnDisable() { PlayerController.clientEntered -= OnClientEntered; }

    void Start()
    {   
        _timeLord = FindObjectOfType<SceneController>().GetTimeLord();
       _endFrame = _startFrame + _growthTime;
       _startPos = gameObject.transform.position;
    }

    void Update()
    {
        // Get the percieved frame.
        float percievedFrame = (float) _timeLord?.GetYourPerceivedFrame();
        Vector3 scaleChange = new Vector3(1f, 1f, 1f);
        Vector3 position = gameObject.transform.position;
        Vector3 positionChange = _startPos;

        if ( (percievedFrame >= _startFrame) && (percievedFrame <= _endFrame) )
        {
            // Get progress modifier from curve.
            float progress = (_endFrame - percievedFrame) / _growthTime;
            float gP = _growthPath.Evaluate(progress);
            // Modifiy values by progress.
            float xzSChange = (gP*_xzScale) + _ScaleBase;
            float ySChange = (gP*_yMax) + _ScaleBase;
            float yPosChange = (gP* _yPosChange);
            scaleChange = new Vector3(xzSChange,ySChange,xzSChange);
            positionChange = _startPos;
            positionChange[1] +=  yPosChange;
        } 
        else if (percievedFrame < _startFrame)
        {
            // Turn collider off.
            _ObjectCollider.isTrigger = true;
            //set origonal scale and pos
            scaleChange = new Vector3(_ScaleBase,_ScaleBase,_ScaleBase);
            positionChange = _startPos;
        } 
        else if (percievedFrame > _startFrame) 
        {
            // Turn collider off.
            _ObjectCollider.isTrigger = false;
            // Set post growth scale and position.
            scaleChange = new Vector3(_xzScale,_yMax,_xzScale);
            positionChange = _startPos;
            positionChange[1] += _yPosChange;
        }
        
        gameObject.transform.position = positionChange;
        gameObject.transform.localScale = scaleChange;

        // Enable/disable hints.
        if (Input.GetKeyDown(Constants.KeyToggleHints)) { _activeHints = !_activeHints; }

        // Set name tag visibility.
        if (_activeHints && _playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            _nameTag.SetActive(_playerTime.CanTimeTravel(Constants.JumpDirection.Backward) && 
                                _startFrame < percievedFrame &&  2 < distance && distance < 10);
        }
        else _nameTag.SetActive(false);
    }

    private void OnClientEntered(PlayerController pc)
    {
        _playerTransform = pc.gameObject.transform;
        _playerTime = pc.gameObject.GetComponent<TimeConn>();
    }
}
