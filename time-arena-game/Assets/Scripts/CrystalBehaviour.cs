using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CrystalBehaviour : MonoBehaviour
{
    public Material overlay;
    public float initial_wave = 0;
    public float offsetY = 0;
    public float offsetRot = 0;
    private float t = 0;
    // Update is called once per frame
    void Update()
    {
      t += Time.deltaTime;
      offsetY = (float)(0.5 * Math.Sin(t))-1;
      gameObject.transform.position = (new Vector3(0.0f, offsetY, 0.0f));
      transform.Rotate(0.0f, 10f*Time.deltaTime, 0.0f, Space.Self);
      overlay.SetFloat("Wave_Incr", 10f*t);
    }
}
