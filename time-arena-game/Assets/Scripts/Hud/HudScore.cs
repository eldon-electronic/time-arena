using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudScore : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private Text _text;
    private int _score;

    void Start()
    {
        _container.SetActive(false);
    }

    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            _container.SetActive(true);
            _text.text = _score + "";
        }
    }

    public void SetScore(int score) { _score = score; }

    public int GetScore() { return _score; }
}
