using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class PlayerHud : MonoBehaviour
{
    private GameController _game;
    [SerializeField] private PhotonView _view;
    [SerializeField] HudStartTimer _startTimer;
    [SerializeField] HudTimeDisplay _timeDisplay;
    [SerializeField] HudMasterClientOptions _masterClientOptions;
    [SerializeField] HudTimeline _timeline;
    [SerializeField] HudCooldowns _cooldowns;
    [SerializeField] HudWinningDisplay _winningDisplay;
    [SerializeField] HudScore _score;
    [SerializeField] HudDebugPanel _debugPanel;
    [SerializeField] HudTutorial _tutorial;
    public Text TeamDispl;

    private float _secondsTillGame;
	private bool _isCountingTillGameStart;

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
            _masterClientOptions.SetActive(false);
            _timeline.SetActive(false);
        }
    }


    // ------------ UPDATE METHODS ------------

    void Update()
    {
        // TODO: Get this out of here!!! This kind of power belongs in GameController or PlayerController at the very least!
        // If counting, reduce timer.
        if (PhotonNetwork.IsMasterClient && _isCountingTillGameStart && _view.IsMine) {
            _secondsTillGame -= Time.deltaTime;
            _masterClientOptions.SetSecondsTillGame(_secondsTillGame);
            if (_secondsTillGame <= 0) {
                PhotonNetwork.LoadLevel("GameScene");
                _isCountingTillGameStart = false;
                _masterClientOptions.SetIsCountingTillStart(_isCountingTillGameStart);
            }
        }
    }


    // ------------ PUBLIC METHODS ------------

    public void setScore(int score) { _score.SetScore(score); }

    public int getScore() { return _score.GetScore(); }

    public void SetTeam(System.String teamName)
    {
        if (_view.IsMine) TeamDispl.text = teamName;
    }

    public void StartCountingDown()
    {
        if (_isCountingTillGameStart) return;

        _isCountingTillGameStart = true;
        _masterClientOptions.SetIsCountingTillStart(_isCountingTillGameStart);
        _secondsTillGame = 5.0f;
        _masterClientOptions.SetSecondsTillGame(_secondsTillGame);
    }

    public void StopCountingDown()
    {
        _isCountingTillGameStart = false;
        _masterClientOptions.SetIsCountingTillStart(_isCountingTillGameStart);
        _secondsTillGame = 0.0f;
        _masterClientOptions.SetSecondsTillGame(_secondsTillGame);
    }

    public void SetDebugValues(Hashtable items)
    {
        _debugPanel.SetDebugValues(items);
    }

    public void SetPlayerPositions(float clientPosition, List<float> playerPositions)
    {
        _timeline.SetPlayerPositions(clientPosition, playerPositions);
    }

    public void SetTimeBarPosition(float position)
    {
        _timeline.SetTimeBarPosition(position);
    }

    public void SetCooldownValues(float[] items)
    {
        _cooldowns.SetCooldownValues(items);
    }

    public void SetTime(int second) { _timeDisplay.SetTime(second); }

    public void ToggleDebug() { _debugPanel.ToggleDebug(); }

    public void PressForwardJumpButton()
    {
        if (!_view.IsMine) return;
        _cooldowns.PressForwardJumpButton();
    }

    public void PressBackJumpButton()
    {
        if (!_view.IsMine) return;
        _cooldowns.PressBackJumpButton();
    }

    public void SetCanJump(bool forward, bool back)
    {
        _cooldowns.SetCanJump(forward, back);
    }

    public void SetArrowPosition(string uiElement)
    {
        if (!_view.IsMine) return;
        _tutorial.SetArrowPosition(uiElement);
    }

    public void SetMessage(string tutorialMessage)
    {
        if (!_view.IsMine) return;
        _tutorial.SetMessage(tutorialMessage);
    }

    public void SetArrowVisibility(bool arrowVisibility)
    {
        if (!_view.IsMine) return;
        _tutorial.SetArrowVisibility(arrowVisibility);
    }

    public void SetTutorialVisibility(bool tutorialVisibility)
    {
        if (!_view.IsMine) return;
        _tutorial.SetVisibility(tutorialVisibility);
    }

    public void SetOptionsText(string optionsMessage)
    {
        if (!_view.IsMine) return;
        _tutorial.SetOptionsText(optionsMessage);
    }
    
    public void SetGame(GameController game)
    {
        _game = game;
        if (_startTimer != null) _startTimer.SetGame(game);
        _timeDisplay.SetGame(game);
        _winningDisplay.SetGame(game);
    }
}
