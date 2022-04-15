using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudStartTimer : MonoBehaviour
{
    [SerializeField] private GameObject _startTimer;
    [SerializeField] private Text _text;
    private GameController _game;

    void OnEnable()
    {
        GameController.gameActive += SetGame;
        GameController.gameStarted += OnGameStart;
    }

    void OnDisable() { GameController.gameActive -= SetGame; }

    void Start() { _startTimer.SetActive(false); }

    void LateUpdate()
    {
        if (_game != null)
        {
            int timer = (int) _game.Timer;
            _text.text = $"{timer}";
        }
    }

    private void SetGame(GameController game)
    {
        _game = game;
        _startTimer.SetActive(true);
    }

    private void OnGameStart() { _startTimer.SetActive(false); }
}
