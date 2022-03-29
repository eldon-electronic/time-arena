using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailController : MonoBehaviour
{
    [SerializeField] private ParticleController _particles;
    private int _playerID;
    private int _tailID;
    private TimeLord _timeLord;
    private bool _activated;
    private bool _amDying;
    
    void Start()
    {
        _activated = false;
        _amDying = false;
    }

    void Update()
    {
        PlayerState state = _timeLord.GetState(_tailID);
        if (state == null) Destroy(gameObject);
        else
        {
            transform.position = state.Pos;
            transform.rotation = state.Rot;

            if (_activated && !_amDying && state.JumpDirection != Constants.JumpDirection.Static)
            {
                _particles.StartDissolving(state.JumpDirection, true);
                _amDying = true;
            }
        }
    }

    // ------------ PUBLIC METHODS ------------

    public void Initialise(PlayerState ps, TimeLord timeLord)
    {
        _playerID = ps.PlayerID;
        _tailID = ps.TailID;
        _timeLord = timeLord;

        transform.position = ps.Pos;
        transform.rotation = ps.Rot;

        if (ps.JumpDirection != Constants.JumpDirection.Static)
        {
            _particles.StartDissolving(ps.JumpDirection, false);
        }

        _activated = true;
    }

    public void Kill() { Destroy(gameObject); }
}
