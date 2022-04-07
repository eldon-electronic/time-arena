using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingControl : MonoBehaviour
{
    //[SerializeField] Volume volume;
    public Volume volume;
    Vignette vignette;
    ChromaticAberration chromaticAbberation;
    LensDistortion lensDistortion;

    public float lensDistortionIntensity = 0;
    public float chromaticAberrationIntensity = 0;
    public float vingetteIntensity = 0;
    public Color vingetteColor = new Color(0, 0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        if(volume.profile.TryGet<Vignette>(out vignette)) {
            vignette.intensity.value = vingetteIntensity;
            vignette.color.value = vingetteColor;
        }
        if(volume.profile.TryGet<ChromaticAberration>(out chromaticAbberation)) {
            chromaticAbberation.intensity.value = chromaticAberrationIntensity;
        }
        if(volume.profile.TryGet<LensDistortion>(out lensDistortion)) {
            lensDistortion.intensity.value = lensDistortionIntensity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        lensDistortion.intensity.value = lensDistortionIntensity;
        chromaticAbberation.intensity.value = chromaticAberrationIntensity;
        vignette.intensity.value = vingetteIntensity;
        vignette.color.value = vingetteColor;
    }
}
