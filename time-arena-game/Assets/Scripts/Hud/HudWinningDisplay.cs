using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudWinningDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _winningDisplay;
    [SerializeField] private Text _text;

    void OnEnable() { GameController.gameEnded += OnGameEnded; }

    void OnDisable() { GameController.gameEnded -= OnGameEnded; }

    private void OnGameEnded(Constants.Team winningTeam)
    {
        _winningDisplay.SetActive(true);
        _text.text = (winningTeam == Constants.Team.Miner) ? "MINERS WIN!" : "GUARDIANS WIN!";
    }
}
