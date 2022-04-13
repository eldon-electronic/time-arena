using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudTimeDisplay : MonoBehaviour
{
    [SerializeField] private Text _text;
    private GameController _game;
    private TimeLord _timeLord;

    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && !_game.GameEnded)
        {
            if (_game.GameStarted)
            {
                float time = _timeLord.GetElapsedTime();
                float t = Constants.GameLength - time;
                int minutes = (int) (t / 60);
                int seconds = (int) (t % 60);
                _text.text = minutes.ToString() + ":" + seconds.ToString().PadLeft(2, '0');
            } else {
                _text.text = "0:00";
            }
        }
    }

    public void SetGame(GameController game) { _game = game; }

    public void SetTimeLord(TimeLord timeLord) { _timeLord = timeLord; }
}
