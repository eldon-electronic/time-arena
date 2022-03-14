using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem FireCircle;
	public ParticleSystem Splash;
  	public Material Material;
    public Animator PlayerAnim;

  	private Color Orange = new Color(1.0f, 0.46f, 0.19f, 1.0f);
  	private Color Blue = new Color(0.19f, 0.38f, 1.0f, 1.0f);
  	private Color White = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    
    void Start()
    {
        Material.SetFloat("_CutoffHeight", 50.0f);
    }

    // Called by code, sets a variable to trigger animation.
    public void StartJumpingForward()
    {
		PlayerAnim.SetBool("isJumpingForward", true);
	}

    // Called by animation, unsets the variable to stop animating.
	public void StopJumpingForward()
    {
		PlayerAnim.SetBool("isJumpingForward", false);
	}

    // Called by code, sets a variable to trigger animation.
	public void StartJumpingBackward()
    {
		PlayerAnim.SetBool("isJumpingBackward", true);
	}

    // Called by animation, unsets the variable to stop animating.
	public void StopJumpingBackward()
    {
		PlayerAnim.SetBool("isJumpingBackward", false);
	}

    // Called by animation, starts the particle systems.
	void BlueBeam()
    {
        var fireCircleMain = FireCircle.main;
        fireCircleMain.startColor = Blue;

        var fireCircleTrails = FireCircle.trails;
        fireCircleTrails.colorOverTrail = Blue;

        var splashMain = Splash.main;
        splashMain.startColor = White;

        var splashTrails = Splash.trails;
        splashTrails.colorOverTrail = Blue;

        FireCircle.Play();
        Splash.Play();
    }

    // Called by animation, starts the particle systems.
    void OrangeBeam()
    {
        var fireCircleMain = FireCircle.main;
        fireCircleMain.startColor = Orange;

        var fireCircleTrails = FireCircle.trails;
        fireCircleTrails.colorOverTrail = Orange;

        var splashMain = Splash.main;
        splashMain.startColor = White;

        var splashTrails = Splash.trails;
        splashTrails.colorOverTrail = Orange;

        FireCircle.Play();
        Splash.Play();
	}
}
