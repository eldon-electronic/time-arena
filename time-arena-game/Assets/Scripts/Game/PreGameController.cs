using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameController : SceneController
{
    private bool _isCountingTillGameStart;
    private float _secondsTillGame;
    public static event Action<float> countDown;

    void Awake()
    {
        _miners = new Dictionary<int, PlayerController>();
		_guardians = new Dictionary<int, PlayerController>();
        _secondsTillGame = 5.0f;
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Constants.FrameRate;

        _timeLord = new TimeLord(Constants.PreGameLength * Constants.FrameRate);
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Input.GetKeyDown(KeyCode.Return)) StartCountingDown();
            if (Input.GetKeyDown(KeyCode.Escape)) StopCountingDown();
        }
        if (_isCountingTillGameStart)
        {
            _secondsTillGame -= Time.deltaTime;
            if (_secondsTillGame <= 0)
            {
                PhotonNetwork.LoadLevel("GameScene");
                _isCountingTillGameStart = false;
            }
            countDown?.Invoke(_secondsTillGame);
        }
        if (_secondsTillGame > 0) _timeLord.Tick();
    }

    private void StartCountingDown()
    {
        if (_isCountingTillGameStart) return;
        _isCountingTillGameStart = true;
        _secondsTillGame = 5.0f;
        countDown?.Invoke(_secondsTillGame);
    }

    private void StopCountingDown()
    {
        _isCountingTillGameStart = false;
        _secondsTillGame = 5.0f;
    }
}
