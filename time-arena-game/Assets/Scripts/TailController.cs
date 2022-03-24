using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailController : MonoBehaviour
{
    [SerializeField] private ParticleController _particles;
    private int _playerID;
    private int _tailID;
    private bool _isDissolving;
    
    void Start() { _isDissolving = false; }

    void Update()
    {
        
    }

    // ------------ PUBLIC METHODS ------------

    public void Initialise(PlayerState ps)
    {
        _playerID = ps.PlayerID;
        _tailID = ps.TailID;

        transform.position = ps.Pos;
        transform.rotation = ps.Rot;

        _particles.StartDissolving(ps.JumpDirection, false);
    }

    public void SetState(PlayerState ps)
    {
        transform.position = ps.Pos;
        transform.rotation = ps.Rot;

        if (!_isDissolving && ps.JumpDirection != Constants.JumpDirection.Static)
        {
            _isDissolving = true;
            _particles.StartDissolving(ps.JumpDirection, true);
        }
    }

    public void Destroy() { Destroy(gameObject); }

    public int GetID() { return _tailID; }
}
