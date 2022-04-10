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
    [SerializeField] private Text _teamDisplay;

    void Start()
    {
        if (_view.IsMine)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                _masterClientOptions.SetActive(false);
            }
        }
        else
        {
            // TODO: After refactoring, remove this with a single command that sets the UI parent object inactive.
            // TODO: Actually, thinking about it, I'm pretty sure we already take care of this in PlayerController...
            // TODO: So the following lines are already redundant...?
            _startTimer.SetActive(false);
            _timeDisplay.SetActive(false);
            _timeline.SetActive(false);
        }
    }


    // ------------ PUBLIC METHODS ------------

    public void setScore(int score) { _score.SetScore(score); }

    public int getScore() { return _score.GetScore(); }

    public void SetTeam(System.String teamName)
    {
        if (_view.IsMine) _teamDisplay.text = teamName;
    }

    public void SetPlayerPositions(float clientPosition, List<float> playerPositions)
    {
        _timeline.SetPlayerPositions(clientPosition, playerPositions);
    }

    public void SetTimeBarPosition(float position)
    {
        _timeline.SetTimeBarPosition(position);
    }

    public void SetTime(int second) { _timeDisplay.SetTime(second); }

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

    public void SetPlayer(PlayerController player)
    {
        _cooldowns.SetPlayer(player);
    }
}
