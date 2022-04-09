using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudTimeDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _timeDisplay;
    [SerializeField] private Text _text;
    private GameController _game;
    private int _time;

    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && !_game.GameEnded)
        {
            if (_game.GameStarted)
            {
                float t = Constants.GameLength - _time;
                int minutes = (int) (t / 60);
                int seconds = (int) (t % 60);
                _text.text = minutes.ToString() + ":" + seconds.ToString().PadLeft(2, '0');
            } else {
                _text.text = "0:00";
            }
        }
    }

    public void SetGame(GameController game) { _game = game; }

    public void Kill() { UnityEngine.Object.Destroy(_timeDisplay); }

    public void SetTime(int second) { _time = second; }
}