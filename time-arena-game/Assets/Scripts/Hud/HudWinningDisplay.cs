using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudWinningDisplay : MonoBehaviour
{
    private GameController _game;
    [SerializeField] private GameObject _winningDisplay;
    [SerializeField] private Text _text;

    void OnEnable()
    {
        GameController.gameEnded += OnGameEnded;
    }

    void OnDisable()
    {
        GameController.gameEnded -= OnGameEnded;
    }

    public void SetGame(GameController game) { _game = game; }

    private void OnGameEnded(Constants.Team winningTeam)
    {
        _winningDisplay.SetActive(true);
        _text.text = (winningTeam == Constants.Team.Miner) ? "MINERS WIN!" : "GUARDIANS WIN!";
    }
}
