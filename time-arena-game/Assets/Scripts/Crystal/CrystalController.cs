using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalController
{
    private Dictionary<int, Vector3[]> positions;
    private Dictionary<int, Quaternion[]> rotations;

    public CrystalController()
    {
        for (int i = 0; i < 13; i++)
        {
            foreach (string line in System.IO.File.ReadLines("crystalShard" + i.ToString() + ".txt"))
            {   
                string[] data = line.Split('|'); 
                string[] _localPositionString = data[0].Split(' ');
                string[] _rotation = data[1].Split(' ');
                if (_localPositionString.Length != 3)
                {
                    throw new System.FormatException("component count mismatch. Expected 3 components but got " + _localPositionString.Length);
                }
                if (_rotation.Length != 4)
                {
                    throw new System.FormatException("component count mismatch. Expected 3 components but got " + _localPositionString.Length);
                }
                Vector3 _localPosition = new Vector3(float.Parse(_localPositionString[0]), float.Parse(_localPositionString[1]), float.Parse(_localPositionString[2]));  
            }  
        }
    }

}
