using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudScore : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _teamScoreText;
    private GameController _game;

    void Start()
    {
        _container.SetActive(false);
    }

    public void setScores(int player, int team){
      _scoreText.text = player + "";
      _teamScoreText.text = team + "";
    }

    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            _container.SetActive(true);
        }
    }

    public void SetGame(GameController game) { _game = game; }
}
