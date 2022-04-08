using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UpdateVolume : MonoBehaviour
{

    public Volume vol;
    public VolumeProfile prof;
    // Start is called before the first frame update
    void Start()
    {
        vol.profile = prof;
    }

    // Update is called once per frame
    void Update()
    {
        vol.profile = prof;
    }
}
