using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem fireCircle;
	public ParticleSystem splash;
  	public Material material;
    public Animator playerAnim;

  	Color ORANGE = new Color(1.0f, 0.46f, 0.19f, 1.0f);
  	Color BLUE = new Color(0.19f, 0.38f, 1.0f, 1.0f);
  	Color WHITE = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    
    void Start()
    {
        material.SetFloat("_CutoffHeight", 50.0f);
    }

    // Called by code, sets a variable to trigger animation
    public void StartJumpingForward()
    {
		playerAnim.SetBool("isJumpingForward", true);
	}

    // Called by animation, unsets the variable to stop animating
	public void StopJumpingForward()
    {
		playerAnim.SetBool("isJumpingForward", false);
	}

    // Called by code, sets a variable to trigger animation
	public void StartJumpingBackward()
    {
		playerAnim.SetBool("isJumpingBackward", true);
	}

    // Called by animation, unsets the variable to stop animating
	public void StopJumpingBackward()
    {
		playerAnim.SetBool("isJumpingBackward", false);
	}

    // Called by animation, starts the particle systems
	void BlueBeam()
    {
        var fcm = fireCircle.main;
        fcm.startColor = BLUE;

        var fct = fireCircle.trails;
        fct.colorOverTrail = BLUE;

        var sm = splash.main;
        sm.startColor = WHITE;

        var st = splash.trails;
        st.colorOverTrail = BLUE;

        fireCircle.Play();
        splash.Play();
    }

    // Called by animation, starts the particle systems
    void OrangeBeam()
    {
        var fcm = fireCircle.main;
        fcm.startColor = ORANGE;

        var fct = fireCircle.trails;
        fct.colorOverTrail = ORANGE;

        var sm = splash.main;
        sm.startColor = WHITE;

        var st = splash.trails;
        st.colorOverTrail = ORANGE;

        fireCircle.Play();
        splash.Play();
	}
}
