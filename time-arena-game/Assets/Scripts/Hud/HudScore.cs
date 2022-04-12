using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudScore : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private Text _text;
    private GameController _game;

    void Start()
    {
        _container.SetActive(false);
    }

    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            _container.SetActive(true);
            _text.text = _game.GetMinerScore() + "";
        }
    }

    public void SetGame(GameController game) { _game = game; }
}
