using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayPPControlScript : PPController
{
    [SerializeField] private VolumeProfile _volProfile;
    [SerializeField] private AnimationCurve _inCurve;
    [SerializeField] private AnimationCurve _outCurve;
    [SerializeField] private PhotonView _view;
    private bool _queued = false;
    private bool _playLock = false;
    private Vignette _vignette;
    private ChromaticAberration _chromaticAbberation;
    private LensDistortion _lensDistortion;
    private float _lensDistortionIntensity = 0.6f;
    private float _chromaticAberrationIntensity = 0.5f;
    private float _vingetteIntensity = 0.6f;
    private Color _vingetteColorForward = new Color(0, 0, 230, 0);
    private Color _vingetteColorBackWard = new Color(230, 100, 0, 0);
    private float _animationDuration = 2;
    private float _lensDist = 0;
    private bool _fadeIn = true;
    private Color _vinColor = new Color(0, 0, 0, 0);
    private float _queuedlensDist = 0;
    private bool _queuedFadeIn = true;
    private Color _queuedvinColor = new Color(0, 0, 0, 0);

    void Awake()
    {
        if (!_view.IsMine) Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!_volProfile.TryGet<Vignette>(out _vignette)) {
            Debug.LogError("Failed to get Vignette");
        }
        if (!_volProfile.TryGet<ChromaticAberration>(out _chromaticAbberation)) {
            Debug.LogError("Failed to get Chrome Ab");
        }
        if (!_volProfile.TryGet<LensDistortion>(out _lensDistortion)) {
            Debug.LogError("Failed to get Distort");
        }
        _vignette.color.value = _vingetteColorBackWard;
        _lensDistortion.intensity.value = 0;
        _chromaticAbberation.intensity.value = 0;
        _vignette.intensity.value = 0;
    }

    public override void TriggerPP(Constants.JumpDirection direction, bool jumpOut)
    {
        if (direction == Constants.JumpDirection.Backward)
        {
            StartAnim(_lensDistortionIntensity, jumpOut, _vingetteColorBackWard);
        }
        else if (direction == Constants.JumpDirection.Forward)
        {
            StartAnim(-1 * _lensDistortionIntensity, jumpOut, _vingetteColorForward);
        }
    }

    //launch animation
    private void StartAnim(float distort, bool fade, Color col) 
    {
        if (_playLock)
        {
            if (!_queued) 
            {
                _queued = true;
                _queuedlensDist = distort;
                _queuedFadeIn = fade;
                _queuedvinColor = col;
            }
        } 
        else
        {
            _playLock = true;
            _lensDist = distort;
            _vinColor = col;
            _fadeIn = fade;
            StartCoroutine(Animate());
        }
        
    }

    //animation coroutine, runs in lockstep with the game (not as a seperate thread)
    IEnumerator Animate()
    {
        float time = 0.0f;
        float duration = _animationDuration;
        _vignette.color.value = _vinColor;
        float modifier = 1;
        while (time < duration)
        {
            if (_fadeIn) modifier = _inCurve.Evaluate(time);
            else modifier = _outCurve.Evaluate(time);
            
            _lensDistortion.intensity.value = modifier * _lensDist;
            _chromaticAbberation.intensity.value = modifier * _chromaticAberrationIntensity;
            _vignette.intensity.value = modifier * _vingetteIntensity;
            time += Time.deltaTime;
            yield return null;
        }
        
        if (_fadeIn) modifier = _inCurve.keys[_inCurve.length-1].value;
        else modifier = _outCurve.keys[_outCurve.length-1].value;
        _lensDistortion.intensity.value = modifier * _lensDist;
        _chromaticAbberation.intensity.value = modifier * _chromaticAberrationIntensity;
        _vignette.intensity.value = modifier * _vingetteIntensity;
        
        if (_queued)
        {
            _lensDist = _queuedlensDist;
            _vinColor = _queuedvinColor;
            _fadeIn = _queuedFadeIn;
            _queued = false;
            StartCoroutine(Animate());
        }
        _playLock = false;
    }
}
