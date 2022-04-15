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
    [SerializeField] HudStartTimer _startTimer;
    [SerializeField] HudTimeDisplay _timeDisplay;
    [SerializeField] HudMasterClientOptions _masterClientOptions;
    [SerializeField] HudTimeline _timeline;
    [SerializeField] HudCooldowns _cooldowns;
    [SerializeField] HudWinningDisplay _winningDisplay;
    [SerializeField] HudScore _score;
    //[SerializeField] HudTutorial _tutorial ;
    [SerializeField] private Text _teamDisplay;

    void Start()
    {
        if (_view.IsMine)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                _masterClientOptions.SetActive(false);
                //_tutorial.SetOptionsText("Go back to tutorial <sprite=1>");
            }
        }
    }


    // ------------ PUBLIC METHODS ------------

    public void SetTeam(System.String teamName)
    {
        if (_view.IsMine) _teamDisplay.text = teamName;
    }

    public void SetScores(int player, int team)
    {
        if (_view.IsMine) _score.setScores(player, team);
    }

    public void SetGame(GameController game)
    {
        if (_view.IsMine)
        {
            _score.SetGame(game);
            if (_startTimer != null) _startTimer.SetGame(game);
            _timeDisplay.SetGame(game);
            _winningDisplay.SetGame(game);
        }
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
