using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CrystalRecorder : MonoBehaviour
{
    
    private Transform shardtransform;
    private static StreamWriter file;

    void Awake()
    {
        file = new StreamWriter(gameObject.name + ".txt");
    }
    
    // Update is called once per frame
    void Update()
    {
        //using StreamWriter file = new StreamWriter("shardPositions.txt");
        shardtransform = this.transform;
        string line = shardtransform.position.ToString() + " " + shardtransform.rotation.ToString();
        file.WriteLine(line);
    }
}
