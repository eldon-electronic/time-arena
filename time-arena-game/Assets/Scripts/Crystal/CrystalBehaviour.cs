using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CrystalBehaviour : MonoBehaviour
{
    public Material overlay;
    public float initial_wave = 0;
    public float offsetRot = 0;
    private float t = 0;

    public int id = 0;
    // Update is called once per frame
    void Update()
    {
      t += Time.deltaTime;
      float offsetY = (float)(0.01 * Math.Sin(t + initial_wave));
      gameObject.transform.Translate(new Vector3(0.0f, offsetY, 0.0f));
      transform.Rotate(0.0f, 30f*Time.deltaTime, 0.0f, Space.Self);

      overlay.SetFloat("Wave_Incr", t);
    }
}
