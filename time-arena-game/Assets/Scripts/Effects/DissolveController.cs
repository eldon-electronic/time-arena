using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DissolveController : DisController
{
    [SerializeField] private AnimationCurve _inCurve;
    [SerializeField] private AnimationCurve _outCurve;
    [SerializeField] private PhotonView _view;
    [SerializeField] private Material _minerMetal;
    [SerializeField] private Material _minerAbdomen;
    [SerializeField] private Material _minerShoe;
    [SerializeField] private Material _minerVisor;
    [SerializeField] private Material _minerEars;
    [SerializeField] private Material _guardianBody;
    [SerializeField] private Material _device;
    [SerializeField] private Material _deviceCompass;
    [SerializeField] private Material _deviceNeedle;
    [SerializeField] private Material _deviceButtonForward;
    [SerializeField] private Material _deviceButtonBackward;
    [SerializeField] private Material _deviceButtonOff;
    [SerializeField] private Material _deviceArrowForward;
    [SerializeField] private Material _deviceArrowBackward;
    [SerializeField] private Material _deviceOffLine;
    
    public Constants.Team Character;
    private float _animationDuration = 3;
    //private float _intensity = 6;
    private Color _backwardColour = new Color(191, 4, 0, 0);
    private Color _forwardColour = new Color(0, 4, 191, 0);
    private Color _disColor = new Color(0, 0, 0, 0);
    private Color _queuedDisColor = new Color(0, 0, 0, 0);
    private bool _fadeIn = true;
    private bool _queuedFadeIn = true;
    private bool _queued = false;
    private bool _playLock = false;

    void Awake()
    {
        if (!_view.IsMine) Destroy(this);
    }


    // Start is called before the first frame update
    void Start()
    {
        _guardianBody.SetColor("_EdgeColour", _backwardColour);
        _minerMetal.SetColor("_EdgeColour",_backwardColour);
        _minerAbdomen.SetColor("_EdgeColour", _backwardColour);
        _minerEars.SetColor("_EdgeColour", _backwardColour);
        _minerShoe.SetColor("_EdgeColour", _backwardColour);
        _minerVisor.SetColor("_EdgeColour", _backwardColour);
        _device.SetColor("_EdgeColour", _backwardColour);
        _deviceArrowBackward.SetColor("_EdgeColour", _backwardColour);
        _deviceOffLine.SetColor("_EdgeColour", _backwardColour);
        _deviceArrowForward.SetColor("_EdgeColour", _backwardColour);
        _deviceButtonBackward.SetColor("_EdgeColour", _backwardColour);
        _deviceButtonOff.SetColor("_EdgeColour", _backwardColour);
        _deviceButtonForward.SetColor("_EdgeColour", _backwardColour);
        _deviceCompass.SetColor("_EdgeColour", _backwardColour);
        _deviceNeedle.SetColor("_EdgeColour",_backwardColour);
        
    }
    public override void TriggerDissolve(Constants.JumpDirection direction, bool jumpOut)
    {
        if (direction == Constants.JumpDirection.Backward)
        {
            StartAnim(jumpOut, _backwardColour);
        }
        else if (direction == Constants.JumpDirection.Forward)
        {
            StartAnim(jumpOut, _forwardColour);
        }
    }

    //launch animation
    private void StartAnim(bool fade, Color col) 
    {
        if (_playLock)
        {
            if (!_queued) 
            {
                _queued = true;
                _queuedFadeIn = fade;
                _queuedDisColor = col;
            }
        } 
        else
        {
            _playLock = true;
            _disColor = col;
            _fadeIn = fade;
            StartCoroutine(AnimateDis());
        }
        
    }
    IEnumerator AnimateDis()
    {
        float time = 0.0f;
        float duration = _animationDuration;
        _guardianBody.SetColor("_EdgeColour", _disColor);
        _minerMetal.SetColor("_EdgeColour",_disColor);
        _minerAbdomen.SetColor("_EdgeColour", _disColor);
        _minerEars.SetColor("_EdgeColour", _disColor);
        _minerShoe.SetColor("_EdgeColour", _disColor);
        _minerVisor.SetColor("_EdgeColour", _disColor);
        _device.SetColor("_EdgeColour", _disColor);
        _deviceArrowBackward.SetColor("_EdgeColour", _disColor);
        _deviceOffLine.SetColor("_EdgeColour", _disColor);
        _deviceArrowForward.SetColor("_EdgeColour", _disColor);
        _deviceButtonBackward.SetColor("_EdgeColour", _disColor);
        _deviceButtonForward.SetColor("_EdgeColour", _disColor);
        _deviceButtonOff.SetColor("_EdgeColour", _disColor);
        _deviceCompass.SetColor("_EdgeColour", _disColor);
        _deviceNeedle.SetColor("_EdgeColour", _disColor);
        float modifier = 1;
        if(Character == Constants.Team.Guardian){
            while (time < duration){
                if (_fadeIn) modifier = _inCurve.Evaluate(time);
                else modifier = _outCurve.Evaluate(time);
                _guardianBody.SetFloat("_CutoffHeight", modifier);
                Debug.Log($"mod: {modifier}");
                Debug.Log($"team: {Character}");
                time += Time.deltaTime;
                yield return null;
            }
            if (_fadeIn) modifier = _inCurve.keys[_inCurve.length-1].value;
            else modifier = _outCurve.keys[_outCurve.length-1].value;
            _guardianBody.SetFloat("_CutoffHeight", modifier);
        }
        else if(Character == Constants.Team.Miner){
            while (time < duration){
                if (_fadeIn) modifier = _inCurve.Evaluate(time);
                else modifier = _outCurve.Evaluate(time);
                _minerMetal.SetFloat("_CutoffHeight", modifier);
                _minerAbdomen.SetFloat("_CutoffHeight", modifier);
                _minerEars.SetFloat("_CutoffHeight", modifier);
                _minerShoe.SetFloat("_CutoffHeight", modifier);
                _minerVisor.SetFloat("_CutoffHeight", modifier);
                _device.SetFloat("_CutoffHeight", modifier);
                _deviceArrowBackward.SetFloat("_CutoffHeight", modifier);
                _deviceArrowForward.SetFloat("_CutoffHeight", modifier);
                _deviceButtonBackward.SetFloat("_CutoffHeight", modifier);
                _deviceButtonForward.SetFloat("_CutoffHeight", modifier);
                _deviceCompass.SetFloat("_CutoffHeight", modifier);
                _deviceNeedle.SetFloat("_CutoffHeight", modifier);
                time += Time.deltaTime;
                yield return null;
            }
            if (_fadeIn) modifier = _inCurve.keys[_inCurve.length-1].value;
            else modifier = _outCurve.keys[_outCurve.length-1].value;
             _minerMetal.SetFloat("_CutoffHeight", modifier);
            _minerAbdomen.SetFloat("_CutoffHeight", modifier);
            _minerEars.SetFloat("_CutoffHeight", modifier);
            _minerShoe.SetFloat("_CutoffHeight", modifier);
            _minerVisor.SetFloat("_CutoffHeight", modifier);
            _device.SetFloat("_CutoffHeight", modifier);
            _deviceArrowBackward.SetFloat("_CutoffHeight", modifier);
            _deviceArrowForward.SetFloat("_CutoffHeight", modifier);
            _deviceButtonBackward.SetFloat("_CutoffHeight", modifier);
            _deviceButtonForward.SetFloat("_CutoffHeight", modifier);
            _deviceCompass.SetFloat("_CutoffHeight", modifier);
            _deviceNeedle.SetFloat("_CutoffHeight", modifier);

        }
        if (_queued)
        {
           
            _disColor = _queuedDisColor;
            _fadeIn = _queuedFadeIn;
            _queued = false;
            StartCoroutine(AnimateDis());
        }
        _playLock = false;
    }
}
