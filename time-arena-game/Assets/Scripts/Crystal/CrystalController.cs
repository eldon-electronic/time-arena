using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class CrystalController : MonoBehaviour
{
    private TextAsset _textFile;
    private List<(Vector3, Quaternion)> _positions;
    private TimeLord _timeLord;
    private int _startFrame;
    private int _animationLength;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Vector3 _endPosition;
    private Quaternion _endRotation;


    void Awake()
    {
        //get the number of the frame that the animation should start at from the parent prefab. 
        _startFrame = transform.parent.gameObject.GetComponent<CrystalParent>().startFrame;
        _textFile = Resources.Load<TextAsset>(gameObject.name);
        _positions = ReadFile();         
        _startPosition = _positions[0].Item1;
        _startRotation = _positions[0].Item2;
        _endPosition = _positions[_positions.Count - 1].Item1;
        _endRotation = _positions[_positions.Count - 1].Item2;
        _animationLength = _positions.Count;
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
        _timeLord = FindObjectOfType<SceneController>().GetTimeLord();
    }

    private void OnNewTimeLord(TimeLord time)
    {
        _timeLord = time;
    } 

    void Update()
    {
        int frame = _timeLord.GetYourFrame();
        // If the current frame being rendered is before the start of the 
        // - animation, then set the local position to default.
        if (frame <= _startFrame)
        {
            transform.localPosition = _startPosition;
            transform.rotation = _startRotation;
        }
        // Else if your current frame is after the animation has ended, then 
        // - set local postion of shard to the final position of the animation. 
        else if (frame >= (_startFrame + _animationLength))
        {
            transform.localPosition = _endPosition;
            transform.rotation = _endRotation;
        }
        else
        {
            transform.localPosition = _positions[frame - _startFrame].Item1;
            transform.rotation = _positions[frame - _startFrame].Item2;
        }
    }

    // ReadFile() parses the file ascociated with the shard and then returns a 
    // - list of the positions and rotations of the shard at each frame of the animation.
    List<(Vector3, Quaternion)> ReadFile()
    {   
        _positions = new List<(Vector3, Quaternion)>();
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
            _positions.Add((localPosition, rotation));
        }
        return _positions;
    }
}
