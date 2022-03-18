using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem FireCircle;
	public ParticleSystem Splash;
  	public Material Material;
    public Animator PlayerAnim;

  	private Color _orange = new Color(1.0f, 0.46f, 0.19f, 1.0f);
  	private Color _blue = new Color(0.19f, 0.38f, 1.0f, 1.0f);
  	private Color _white = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private bool _isJumping;
    
    void Start()
    {
        Material.SetFloat("_CutoffHeight", 50.0f);

        _isJumping = false;
    }

    // Called by code, sets a variable to trigger animation.
    public void StartJumpingForward()
    {
        _isJumping = true;
		PlayerAnim.SetBool("isJumpingForward", true);
	}

    // Called by animation, unsets the variable to stop animating.
	public void StopJumpingForward()
    {
        _isJumping = false;
		PlayerAnim.SetBool("isJumpingForward", false);
	}

    // Called by code, sets a variable to trigger animation.
	public void StartJumpingBackward()
    {
        _isJumping = true;
		PlayerAnim.SetBool("isJumpingBackward", true);
	}

    // Called by animation, unsets the variable to stop animating.
	public void StopJumpingBackward()
    {
        _isJumping = false;
		PlayerAnim.SetBool("isJumpingBackward", false);
	}

    public bool IsJumping() { return _isJumping; }

    // Called by animation, starts the particle systems.
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

    // Called by animation, starts the particle systems.
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
}
