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
    private bool _isCountingTillGameStart;
    private float _secondsTillGame;

    private void LateUpdate()
    {
        // If master client, show options message.
        _masterClientOptions.SetActive(SceneManager.GetActiveScene().name == "PreGameScene");

        if (_isCountingTillGameStart)
        {
            var timeLeft = System.Math.Round(_secondsTillGame, 0);
            _text.text = $"Starting in {timeLeft}s";
            if (System.Math.Round(_secondsTillGame, 0) <= 0.0f)
            {
                _text.text = "Loading...";
            }
        } else {
          _text.text = "Press F to Start";
        }
    }

    public void SetActive(bool value) { _masterClientOptions.SetActive(value); }

    public void SetIsCountingTillStart(bool value) { _isCountingTillGameStart = value; }

    public void SetSecondsTillGame(float seconds) { _secondsTillGame = seconds; }
}
