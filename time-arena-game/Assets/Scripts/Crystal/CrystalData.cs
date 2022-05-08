using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalData : MonoBehaviour
{
    private TextAsset _textFile;
    public List<(Vector3, Quaternion)> positions;

    void Awake()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
        //get the number of the frame that the animation should start at from the parent prefab. 
        _textFile = Resources.Load<TextAsset>(gameObject.name.Replace("data", ""));
        positions = ReadFile();  
    }
    


    List<(Vector3, Quaternion)> ReadFile()
    {   
        positions = new List<(Vector3, Quaternion)>();
        foreach (string line in _textFile.text.Split('\n'))
        {   
            string[] data = line.Split('|'); 
            string[] localPositionString = data[0].Split(' ');
            string[] rotationString = data[1].Split(' ');
            if (localPositionString.Length != 3)
            {
                throw new System.FormatException("component count mismatch. Expected 3 components but got " + localPositionString.Length);
            }
            if (rotationString.Length != 4)
            {
                throw new System.FormatException("component count mismatch. Expected 3 components but got " + rotationString.Length);
            }
            Vector3 localPosition = new Vector3(float.Parse(localPositionString[0]), float.Parse(localPositionString[1]), float.Parse(localPositionString[2]));
            Quaternion rotation = new Quaternion(float.Parse(rotationString[1]), float.Parse(rotationString[2]), float.Parse(rotationString[3]), float.Parse(rotationString[0])); 
            positions.Add((localPosition, rotation));
        }
        return positions;
    }
}
