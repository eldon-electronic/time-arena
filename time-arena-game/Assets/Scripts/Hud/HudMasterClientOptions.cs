using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HudMasterClientOptions : MonoBehaviour
{
    [SerializeField] private GameObject _masterClientOptions;
    [SerializeField] private TMP_Text _text;

    void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _masterClientOptions.SetActive(false);
            Destroy(this);
        }
    }

    void OnEnable()
    {
        PreGameController.countDown += OnCountDown;
        GameController.gameActive += OnGameActive;
    }

    void OnDisable()
    {
        PreGameController.countDown -= OnCountDown;
        GameController.gameActive -= OnGameActive;
    }

    void Start()
    {
        _text.text = "Press <sprite=8> to Start";
    }

    private void OnCountDown(float secondsTillGame)
    {
        var timeLeft = System.Math.Round(secondsTillGame, 0);
        if (System.Math.Round(secondsTillGame, 0) > 0.0f)
        {
            _text.text = $"Starting in {timeLeft}s";
        }
        else _text.text = "Loading...";
    }

    private void OnGameActive(GameController game)
    {
        _masterClientOptions.SetActive(false);
    }
}
