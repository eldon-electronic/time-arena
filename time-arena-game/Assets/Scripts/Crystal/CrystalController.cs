using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CrystalController : MonoBehaviour
{
    private List<(Vector3, Quaternion)> _positions;

    private TimeLord _timeLord;

    private int startFrame;

    void Awake()
    {
        //get the number of the frame that the animation should start at from the parent prefab. 
        startFrame = transform.parent.gameObject.GetComponent<CrystalParent>().startFrame;
        _positions = new List<(Vector3, Quaternion)>();
        foreach (string line in System.IO.File.ReadLines(gameObject.name + ".txt"))
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
            Quaternion rotation = new Quaternion(float.Parse(rotationString[0]), float.Parse(rotationString[1]), float.Parse(rotationString[2]), float.Parse(rotationString[3])); 
            _positions.Add((localPosition, rotation));
        }  
        
    }

    void OnEnable()
    {
        GameController.newTimeLord += OnNewTimeLord;
    }
    
    void OnDisable()
    {
        GameController.newTimeLord -= OnNewTimeLord;
    }

    void Start()
    {
        _timeLord = GameObject.FindWithTag("PreGameController").GetComponent<PreGameController>().GetTimeLord();
    }

    private void OnNewTimeLord(TimeLord time)
    {
        _timeLord = time;
    } 

    void Update()
    {
        int frame = _timeLord.getYourFrame();
        if(frame <= startFrame)
        {
            transform.localPosition = _positions[0];
        }
    }
}
