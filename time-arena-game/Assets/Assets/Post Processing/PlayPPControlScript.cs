using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayPPControlScript : MonoBehaviour
{
    public VolumeProfile volProfile;
    public AnimationCurve inCurve;
    public AnimationCurve outCurve;
    bool queued = false;
    bool playLock = false;
    Scene lastScene;    
    Vignette vignette;
    ChromaticAberration chromaticAbberation;
    LensDistortion lensDistortion;

    public float lensDistortionIntensity = 0.6f;
    public float chromaticAberrationIntensity = 0.5f;
    public float vingetteIntensity = 0.6f;
    public Color vingetteColorForward = new Color(0, 0, 230, 0);
    public Color vingetteColorBackWard = new Color(230, 100, 0, 0);
    public float animationDuration = 2;
    float lensDist = 0;
    bool fadeIn = true;
    Color vinColor = new Color(0, 0, 0, 0);
    float queuedlensDist = 0;
    bool queuedFadeIn = true;
    Color queuedvinColor = new Color(0, 0, 0, 0);



    // Start is called before the first frame update
    void Start()
    {
        if(volProfile.TryGet<Vignette>(out vignette)) {
            Debug.Log("Got Vignette");
        }
        if(volProfile.TryGet<ChromaticAberration>(out chromaticAbberation)) {
            Debug.Log("Got Chrome Ab");
        }
        if(volProfile.TryGet<LensDistortion>(out lensDistortion)) {
            Debug.Log("Got Distort");
        }
        vignette.color.value = vingetteColorBackWard;
        lensDistortion.intensity.value = 0;
        chromaticAbberation.intensity.value = 0;
        vignette.intensity.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
            //back start
            if (Input.GetKeyDown(KeyCode.Q)) {
                StartAnim(lensDistortionIntensity, true, vingetteColorBackWard);               
            }

            //back fin
            if (Input.GetKeyUp(KeyCode.Q)) {
                StartAnim(lensDistortionIntensity, false, vingetteColorBackWard); 
            }

            //forward start
            if (Input.GetKeyDown(KeyCode.E)) {
                StartAnim(-1 *lensDistortionIntensity, true, vingetteColorForward);
            }

            //forward fin
            if (Input.GetKeyUp(KeyCode.E)) {
                StartAnim(-1 *lensDistortionIntensity, false, vingetteColorForward);
            }
        
    }

    //launch animation
    public void StartAnim(float distort, bool fade, Color col) 
    {
        //check lock
        if(playLock)
        {
            if(!queued) 
            {
                queued = true;
                queuedlensDist = distort;
                queuedFadeIn = fade;
                queuedvinColor = col;
            }
        } 
        else
        {
            playLock = true;
            lensDist = distort;
            vinColor = col;
            fadeIn = fade;
            StartCoroutine(Animate());
        }
        
    }

    //animation coroutine, runs in lockstep with the game (not as a seperate thread)
    IEnumerator Animate()
    {
        float time = 0.0f;
        float duration = animationDuration;
        vignette.color.value = vinColor;
        float modifier = 1;
        while (time < duration)
        {
            if(fadeIn)
            {
                modifier = inCurve.Evaluate(time);
            }
            else
            {
                modifier = outCurve.Evaluate(time);
            }
            
            lensDistortion.intensity.value = modifier * lensDist;
            chromaticAbberation.intensity.value = modifier * chromaticAberrationIntensity;
            vignette.intensity.value = modifier * vingetteIntensity;
            time += Time.deltaTime;
            yield return null;
        }
        
        if(fadeIn)
            {
                modifier = inCurve.keys[inCurve.length-1].value;
            }
        else
            {
                modifier = outCurve.keys[outCurve.length-1].value;
            }
        lensDistortion.intensity.value = modifier * lensDist;
        chromaticAbberation.intensity.value = modifier * chromaticAberrationIntensity;
        vignette.intensity.value = modifier * vingetteIntensity;
        

        if(queued)
        {
            lensDist = queuedlensDist;
            vinColor = queuedvinColor;
            fadeIn = queuedFadeIn;
            queued = false;
            StartCoroutine(Animate());
        }
        playLock = false;
    }
}
