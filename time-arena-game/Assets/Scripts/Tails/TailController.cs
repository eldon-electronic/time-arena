using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class TailController : MonoBehaviour
{
    [SerializeField] private ParticleController _particles;
    [SerializeField] private TextMeshProUGUI nameText;
    private int _playerID;
    private int _tailID;
    private TimeLord _timeLord;
    private TailManager _manager;


    void Update()
    {
        PlayerState state = _timeLord.GetTailState(_tailID);
        if (state == null) _manager.ReceiveResignation(_tailID);
        else
        {
            if (state.Pos != transform.position) transform.position = state.Pos;
            if (state.Rot != transform.rotation) transform.rotation = state.Rot;

            if (state.JumpDirection != Constants.JumpDirection.Static && _manager.GetParticlesEnabled())
            {
                // _particles.StartDissolving(state.JumpDirection, state.JumpingOut);
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
            // _particles.StartDissolving(ps.JumpDirection, false);
        }

        nameText.text = PhotonView.Find(_playerID).Owner.NickName;
        
        Constants.Team team;
        if (SceneManager.GetActiveScene().name == "PreGameScene")
        {
            team = FindObjectOfType<PreGameController>().GetComponent<PreGameController>().GetTeam(_playerID);
        }
        else team = FindObjectOfType<GameController>().GetComponent<GameController>().GetTeam(_playerID);
        // TODO: use the team to set visibility of mesh.
    }

    // Tails must be ordered to commit suicide; they should not be able to take the initiative themselves.
    public void Kill()
    {
        UnityEngine.Object.Destroy(gameObject);
    }
}
