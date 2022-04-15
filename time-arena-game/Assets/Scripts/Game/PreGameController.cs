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
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Constants.FrameRate;

        _timeLord = new TimeLord(Constants.PreGameLength * Constants.FrameRate);
    }

    void Update()
    {
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
        _timeLord.Tick();
    }

    public void Register(PlayerController pc)
    {
        pc.SetTimeLord(_timeLord);
        int id = pc.GetID();
		if (pc.Team == Constants.Team.Guardian) _guardians.Add(id, pc);
		else _miners.Add(id, pc);
    }

    public void StartCountingDown()
    {
        if (_isCountingTillGameStart) return;
        _isCountingTillGameStart = true;
        _secondsTillGame = 5.0f;
        countDown?.Invoke(_secondsTillGame);
    }

    public void StopCountingDown()
    {
        _isCountingTillGameStart = false;
        _secondsTillGame = 0.0f;
    }
}
