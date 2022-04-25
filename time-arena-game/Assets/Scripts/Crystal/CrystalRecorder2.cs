using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class CrystalRecorder2 : MonoBehaviour
{
    
    private Transform _shardtransform;
    private StreamWriter _file;

    void Awake()
    {
        _file = new StreamWriter(gameObject.name + ".txt");
    }
    
    // Update is called once per frame
    void Update()
    {
        _shardtransform = this.transform;
        StringBuilder sb = new StringBuilder();
        sb.Append(_shardtransform.localPosition.x).Append(" ").Append(_shardtransform.localPosition.y).Append(" ").Append(_shardtransform.localPosition.z).Append("|").Append(_shardtransform.rotation.w).Append(" ").Append(_shardtransform.rotation.x).Append(" ").Append(_shardtransform.rotation.y).Append(" ").Append(_shardtransform.rotation.z);
        string line = sb.ToString();
        _file.WriteLine(line);
    }
}