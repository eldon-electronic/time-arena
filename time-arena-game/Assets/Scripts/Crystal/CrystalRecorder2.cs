using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CrystalRecorder2 : MonoBehaviour
{
    
    private Transform shardtransform;
    private StreamWriter file;

    void Awake()
    {
        file = new StreamWriter(gameObject.name + ".txt");
    }
    
    // Update is called once per frame
    void Update()
    {
        shardtransform = this.transform;
        string line = shardtransform.localPosition.ToString() + " " + shardtransform.localRotation.ToString();
        file.WriteLine(line);
    }
}