using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBehaviour : MonoBehaviour
{
    public Material overlay;
    public float initial_wave = 0;
    public float offsetY = 0;
    public float offsetRot = 0;
    // Update is called once per frame
    void Update()
    {
      overlay.SetFloat("Wave_Incr", initial_wave+0.001f);

    }
}
