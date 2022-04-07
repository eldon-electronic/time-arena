using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingControl : MonoBehaviour
{
    public Volume volume;
    public AnimationCurve curve;
    Vignette vignette;
    ChromaticAberration chromaticAbberation;
    LensDistortion lensDistortion;

    public float lensDistortionIntensity = 0.6f;
    public float chromaticAberrationIntensity = 0.5f;
    public float vingetteIntensity = 0.6f;
    public Color vingetteColor = new Color(0, 0, 230, 0);
    public float animationDuration;

    // Start is called before the first frame update
    void Start()
    {
        if(volume.profile.TryGet<Vignette>(out vignette)) {
            //vignette.intensity.value = vingetteIntensity; 
            //vignette.color.value = vingetteColor;
        }
        if(volume.profile.TryGet<ChromaticAberration>(out chromaticAbberation)) {
            //chromaticAbberation.intensity.value = chromaticAberrationIntensity;
        }
        if(volume.profile.TryGet<LensDistortion>(out lensDistortion)) {
            //lensDistortion.intensity.value = lensDistortionIntensity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //lensDistortion.intensity.value = lensDistortionIntensity;
        ///chromaticAbberation.intensity.value = chromaticAberrationIntensity;
        //vignette.intensity.value = vingetteIntensity;
       //vignette.color.value = vingetteColor;
    }

    public void StartAnim() 
    {
        StartCoroutine(Animate());
        Debug.LogError("playing Anim");
    }

    //animation coroutine
    IEnumerator Animate()
    {
        float time = 0.0f;
        float duration = animationDuration;
        vignette.color.value = vingetteColor;
        while (time < duration)
        {
            float modifier = curve.Evaluate(time);
            lensDistortion.intensity.value = modifier * lensDistortionIntensity;
            chromaticAbberation.intensity.value = modifier * chromaticAberrationIntensity;
            vignette.intensity.value = modifier * vingetteIntensity;
            time += Time.deltaTime;
            yield return null;
        }
        float modifier2 = curve.keys[curve.length-1].value;
            lensDistortion.intensity.value = modifier2 * lensDistortionIntensity;
            chromaticAbberation.intensity.value = modifier2 * chromaticAberrationIntensity;
            vignette.intensity.value = modifier2 * vingetteIntensity;
    }
}
