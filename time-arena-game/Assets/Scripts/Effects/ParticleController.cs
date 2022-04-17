using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: revert back to this original method (to avoid alerting everyone)
public interface ParticleUser
{
    public void NotifyStartedDissolving();
    public void NotifyStoppedDissolving(bool dissolvedOut);
}


public class ParticleController : MonoBehaviour
{
    public ParticleSystem FireCircle;
	public ParticleSystem Splash;
  	public Material Material;
    public Animator PlayerAnim;
    private ParticleUser _subscriber;
  	private Color _orange = new Color(1.0f, 0.46f, 0.19f, 1.0f);
  	private Color _blue = new Color(0.19f, 0.38f, 1.0f, 1.0f);
  	private Color _white = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    
    void Awake()
    {
        Material.SetFloat("_CutoffHeight", 50.0f);
    }


    // ------------ HELPER FUNCTIONS ------------

    private void SetDissolveAnimationVariable(Constants.JumpDirection jd, bool dissolveOut, bool active)
    {
        if (jd == Constants.JumpDirection.Forward && dissolveOut)
        {
            PlayerAnim.SetBool("isDissolvingForwardOut", active);
        }
        else if (jd == Constants.JumpDirection.Forward && !dissolveOut)
        {
            PlayerAnim.SetBool("isDissolvingForwardIn", active);
        }
        else if (jd == Constants.JumpDirection.Backward && dissolveOut)
        {
            PlayerAnim.SetBool("isDissolvingBackwardOut", active);
        }
        else if (jd == Constants.JumpDirection.Backward && !dissolveOut)
        {
            PlayerAnim.SetBool("isDissolvingBackwardIn", active);
        }
    }

    // TODO: Remove isJumpingForward=false from being the condition for leaving the animation state, make it happen unconditionally if possible.
    // TODO: An alternative may be to have separate Animation systems (for grabbing, walking, jumping, etc., allowing multiple to happen simultaneously).
    private void StopDissolving(Constants.JumpDirection jd, bool dissolveOut)
    {
        SetDissolveAnimationVariable(jd, dissolveOut, false);
        _subscriber?.NotifyStoppedDissolving(dissolveOut);
    }


    // ------------ PUBLIC METHODS ------------

    public void StartDissolving(Constants.JumpDirection jd, bool dissolveOut)
    {
        SetDissolveAnimationVariable(jd, dissolveOut, true);
    }

    public void SetSubscriber(ParticleUser subscriber) { _subscriber = subscriber; }


    // ------------ FUNCTIONS CALLED BY ANIMATION ------------

    // Starts the blue particle systems.
	void BlueBeam()
    {
        var fireCircleMain = FireCircle.main;
        fireCircleMain.startColor = _blue;

        var fireCircleTrails = FireCircle.trails;
        fireCircleTrails.colorOverTrail = _blue;

        var splashMain = Splash.main;
        splashMain.startColor = _white;

        var splashTrails = Splash.trails;
        splashTrails.colorOverTrail = _blue;

        FireCircle.Play();
        Splash.Play();
    }

    // Starts the orange particle systems.
    void OrangeBeam()
    {
        var fireCircleMain = FireCircle.main;
        fireCircleMain.startColor = _orange;

        var fireCircleTrails = FireCircle.trails;
        fireCircleTrails.colorOverTrail = _orange;

        var splashMain = Splash.main;
        splashMain.startColor = _white;

        var splashTrails = Splash.trails;
        splashTrails.colorOverTrail = _orange;

        FireCircle.Play();
        Splash.Play();
	}

    void StopDissolvingBackOut() { StopDissolving(Constants.JumpDirection.Backward, true); }

    void StopDissolvingBackIn() { StopDissolving(Constants.JumpDirection.Backward, false); }

    void StopDissolvingForwardOut() { StopDissolving(Constants.JumpDirection.Forward, true); }

    void StopDissolvingForwardIn() { StopDissolving(Constants.JumpDirection.Forward, false); }

    void StartedDissolving() { _subscriber?.NotifyStartedDissolving(); }
}
