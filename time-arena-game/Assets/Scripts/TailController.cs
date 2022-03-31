using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailController : MonoBehaviour
{
    [SerializeField] private ParticleController _particles;
    private int _playerID;
    private int _tailID;
    private TimeLord _timeLord;
    private TailManager _manager;
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
        if (state == null) _manager.ReceiveResignation(_tailID);
        else
        {
            transform.position = state.Pos;
            transform.rotation = state.Rot;

            if (state.JumpDirection != Constants.JumpDirection.Static && _manager.GetParticlesEnabled())
            {
                _particles.StartDissolving(state.JumpDirection, state.JumpingOut);
            }
        }
    }

    // ------------ PUBLIC METHODS ------------

    public void Initialise(PlayerState ps, TimeLord timeLord, TailManager manager)
    {
        _playerID = ps.PlayerID;
        _tailID = ps.TailID;
        _timeLord = timeLord;
        _manager = manager;

        transform.position = ps.Pos;
        transform.rotation = ps.Rot;

        if (ps.JumpDirection != Constants.JumpDirection.Static)
        {
            _particles.StartDissolving(ps.JumpDirection, false);
        }

        _activated = true;
    }

    public void Kill()
    {
        UnityEngine.Object.Destroy(gameObject);
    }
}
