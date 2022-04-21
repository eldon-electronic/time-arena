using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

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
        StringBuilder sb = new StringBuilder();
        sb.Append(shardtransform.localPosition.x).Append(" ").Append(shardtransform.localPosition.y).Append(" ").Append(shardtransform.localPosition.z).Append("|").Append(shardtransform.rotation.w).Append(" ").Append(shardtransform.rotation.x).Append(" ").Append(shardtransform.rotation.y).Append(" ").Append(shardtransform.rotation.z);
        string line = sb.ToString();
        file.WriteLine(line);
    }
}