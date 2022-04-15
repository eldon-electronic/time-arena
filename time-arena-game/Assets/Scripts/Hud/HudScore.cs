using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudScore : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private Text _text;

    void OnEnable()
    {
        GameController.gameActive += GameActive;
        SceneController.scoreChange += SetScore;
    }

    void OnDisable()
    {
        GameController.gameActive -= GameActive;
        SceneController.scoreChange += SetScore;
    }

    void Start() { _container.SetActive(false); }

    private void GameActive(GameController game) { _container.SetActive(true); }

    private void SetScore(int score) { _text.text = score + ""; }
}
