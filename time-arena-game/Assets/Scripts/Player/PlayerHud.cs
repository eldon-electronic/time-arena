using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class PlayerHud : MonoBehaviour
{
    [SerializeField] private PhotonView _view;
    [SerializeField] HudTimeDisplay _timeDisplay;
    [SerializeField] HudTimeline _timeline;
    [SerializeField] HudCooldowns _cooldowns;
    [SerializeField] private Text _teamDisplay;


    // ------------ PUBLIC METHODS ------------

    public void SetTeam(System.String teamName)
    {
        if (_view.IsMine) _teamDisplay.text = teamName;
    }

    public void SetTimeLord(TimeLord timeLord)
    {
        _timeDisplay.SetTimeLord(timeLord);
        _timeline.SetTimeLord(timeLord);
    }

    public void SetPlayer(PlayerController player)
    {
        _cooldowns.SetPlayer(player);
    }
}
