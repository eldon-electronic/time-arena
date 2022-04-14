using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameController : SceneController
{
    private Dictionary<int, PlayerController> _players;
    private bool _isCountingTillGameStart;
    private float _secondsTillGame;

    void Awake()
    {
        _miners = new Dictionary<int, PlayerController>();
		_guardians = new Dictionary<int, PlayerController>();

        int totalFrames = Constants.FrameRate * Constants.PreGameLength;
        _timeLord = new TimeLord(totalFrames);
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Constants.FrameRate;
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
        }
        _timeLord.Tick();
    }

    public void Register(PlayerController pc)
    {
        pc.SetTimeLord(_timeLord);

        int id = pc.GetID();
		if (pc.Team == Constants.Team.Guardian) _guardians.Add(id, pc);
		else _miners.Add(id, pc);

        Debug.Log($"id: {id}, team: {pc.Team}");
    }

    public void StartCountingDown()
    {
        if (_isCountingTillGameStart) return;
        _isCountingTillGameStart = true;
        _secondsTillGame = 5.0f;
    }

    public void StopCountingDown()
    {
        _isCountingTillGameStart = false;
        _secondsTillGame = 0.0f;
    }

    public bool IsCountingDown() { return _isCountingTillGameStart; }

    public float GetSecondsTillGame() { return _secondsTillGame; }
}
