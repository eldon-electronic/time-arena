using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailController : MonoBehaviour
{
    [SerializeField] private ParticleController _particles;
    private int _playerID;
    private int _tailID;
    private bool _isDissolving;
    private TimeLord _timeLord;
    
    void Start() { _isDissolving = false; }

    void Update()
    {
        PlayerState state = _timeLord.GetState(_tailID);
        if (state == null) Destroy(gameObject);
        else
        {
            transform.position = state.Pos;
            transform.rotation = state.Rot;

            if (!_isDissolving && state.JumpDirection != Constants.JumpDirection.Static)
            {
                _isDissolving = true;
                _particles.StartDissolving(state.JumpDirection, true);
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
    }

    public void Kill() { Destroy(gameObject); }
}
