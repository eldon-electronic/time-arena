using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudScore : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private Text _text;
    private SceneController _sceneController;

    void Start()
    {
        _container.SetActive(false);

        GameObject pregame = GameObject.FindWithTag("PreGameController");
        _sceneController = pregame.GetComponent<PreGameController>();

        SceneManager.activeSceneChanged += OnSceneChange;
    }

    void LateUpdate()
    {
        _text.text = _sceneController.GetMinerScore() + "";
    }

    private void OnSceneChange(Scene current, Scene next)
    {
        _container.SetActive(true);

        GameObject game = GameObject.FindWithTag("GameController");
        _sceneController = game.GetComponent<GameController>();
    }
}
