using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface DissolveUser
{
    public void NotifyStartedDissolving();
    public void NotifyStoppedDissolving(bool dissolvedOut);
    public Constants.Team GetTeam();
}


public class DissolveController : DisController
{
    [SerializeField] private AnimationCurve _inCurve;
    [SerializeField] private AnimationCurve _outCurve;
    [SerializeField] private PhotonView _view;
    
    private float _animationDuration = 3;
    private Color _backwardColour = new Color(191, 4, 0, 0);
    private Color _forwardColour = new Color(0, 4, 191, 0);
    private Color _disColor;
    private bool _fadeIn;
    private DissolveUser _subscriber;

    void Awake()
    {
        if (_view.IsMine) Destroy(this);
    }

    public void SetSubscriber(DissolveUser subscriber) { _subscriber = subscriber; }

    public override void TriggerDissolve(Constants.JumpDirection direction, bool jumpOut)
    {
        _fadeIn = !jumpOut;

        if (direction == Constants.JumpDirection.Backward) _disColor = _backwardColour;
        else _disColor = _forwardColour;

        StartCoroutine(AnimateDis());
    }

    private void SetColour(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null) renderer.material.SetColor("_EdgeColour", _disColor);

        for (int i=0; i < obj.transform.childCount; i++)
        {
            SetColour(obj.transform.GetChild(i).gameObject);
        }
    }

    private void SetCutOffHeight(GameObject obj, float modifier)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null) renderer.material.SetFloat("_CutoffHeight", modifier);

        for (int i=0; i < obj.transform.childCount; i++)
        {
            SetCutOffHeight(obj.transform.GetChild(i).gameObject, modifier);
        }
    }

    IEnumerator AnimateDis()
    {
        _subscriber?.NotifyStartedDissolving();

        float time = 0.0f;

        SetColour(gameObject);

        float modifier;

        while (time < _animationDuration)
        {
            if (_fadeIn) modifier = _inCurve.Evaluate(time);
            else modifier = _outCurve.Evaluate(time);

            SetCutOffHeight(gameObject, modifier);

            time += Time.deltaTime;

            yield return null;
        }

        if (_fadeIn) modifier = _inCurve.keys[_inCurve.length-1].value;
        else modifier = _outCurve.keys[_outCurve.length-1].value;

        SetCutOffHeight(gameObject, modifier);

        _subscriber?.NotifyStoppedDissolving(!_fadeIn);
    }
}
