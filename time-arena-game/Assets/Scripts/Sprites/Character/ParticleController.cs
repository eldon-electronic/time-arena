using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParticleController : MonoBehaviour
{
    public ParticleSystem FireCircle;
	public ParticleSystem Splash;
    public ParticleSystem DroppingCrystals;


  	private Color _orange = new Color(1.0f, 0.46f, 0.19f, 1.0f);
  	private Color _blue = new Color(0.19f, 0.38f, 1.0f, 1.0f);
  	private Color _white = new Color(1.0f, 1.0f, 1.0f, 1.0f);


    // ------------ FUNCTIONS CALLED BY ANIMATION ------------

    // Starts the blue particle systems.
	private void BlueBeam()
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
    private void OrangeBeam()
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

    public void StartParticles(Constants.JumpDirection direction)
    {
        if (direction == Constants.JumpDirection.Forward)
        {
            BlueBeam();
        }
        else OrangeBeam();
    }
    public void DropCrystal(){
        DroppingCrystals.Play();
    }
}
