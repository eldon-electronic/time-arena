using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleEffects : MonoBehaviour
{

    public ParticleSystem fireCircle;
    public ParticleSystem splash;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    // TODO: this is for testing purposes only - remove later
    void Update()
    {
        if ( Input.GetKeyDown( KeyCode.J ) )
        {
            TimeJump();
        }
    }

    void TimeJump()
    {
        fireCircle.Play();
        splash.Play();
    }
}
