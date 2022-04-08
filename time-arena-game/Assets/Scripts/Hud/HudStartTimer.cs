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

    void Start()
    {
        _startTimer.SetActive(false);
    }

    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            _startTimer.SetActive(!_game.GameStarted);
            if (!_game.GameStarted && !_game.GameEnded)
            {
                int timer = (int) _game.Timer;
                _text.text = $"{timer}";
            }
        }
    }

    public void SetGame(GameController game) { _game = game; }

    public void Kill() { UnityEngine.Object.Destroy(_startTimer); }
}
