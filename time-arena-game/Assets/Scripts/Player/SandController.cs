using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandController : MonoBehaviour
{
    [SerializeField] private Material _sandMat;
    private Dictionary<Constants.JumpDirection, Vector2> _speed;
    
    void Awake()
    {
        _speed = new Dictionary<Constants.JumpDirection, Vector2>{
            {Constants.JumpDirection.Backward, new Vector2(0f, -1f)},
            {Constants.JumpDirection.Forward, new Vector2(0f, 1f)},
            {Constants.JumpDirection.Static, new Vector2(0f, 0.2f)}
        };
        SetDirection(Constants.JumpDirection.Static);
    }

    public void SetDirection(Constants.JumpDirection direction)
    {
        _sandMat.SetVector("Sand_Direction", _speed[direction]);
        _sandMat.SetVector("Noise_Direction", _speed[direction]);
        _sandMat.SetVector("Voronoi_Direction", _speed[direction]);
    }
}
