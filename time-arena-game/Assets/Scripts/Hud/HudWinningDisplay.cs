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

    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && _game.GameEnded)
        {
            _winningDisplay.SetActive(true);
            _text.text = (_game.WinningTeam == Constants.Team.Miner) ? "MINERS WIN!" : "GUARDIANS WIN!";
        }
    }

    public void SetGame(GameController game) { _game = game; }
}
