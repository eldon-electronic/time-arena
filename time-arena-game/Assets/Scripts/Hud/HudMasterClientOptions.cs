using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudMasterClientOptions : MonoBehaviour
{
    [SerializeField] private GameObject _masterClientOptions;
    [SerializeField] private Text _text;
    private PreGameController _preGame;

    void Start()
    {
        _preGame = FindObjectOfType<PreGameController>();
        if (_preGame == null) Debug.LogError("PreGameController not found");
    }

    private void LateUpdate()
    {
        // If master client, show options message.
        if (SceneManager.GetActiveScene().name == "PreGameScene" && Input.GetKeyDown(KeyCode.Return))
        {
            _masterClientOptions.SetActive(true);
        }

        if (_preGame != null && _preGame.IsCountingDown())
        {
            float secondsTillGame = _preGame.GetSecondsTillGame();
            var timeLeft = System.Math.Round(secondsTillGame, 0);
            _text.text = $"Starting in {timeLeft}s";
            if (System.Math.Round(secondsTillGame, 0) <= 0.0f)
            {
                _text.text = "Loading...";
            }
        }
        //else _text.text = "Press F to Start";
        else _masterClientOptions.SetActive(false);
    }

    public void SetActive(bool value) { _masterClientOptions.SetActive(value); }
}
