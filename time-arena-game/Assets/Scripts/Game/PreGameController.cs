using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameController : MonoBehaviour
{
    private TimeLord _timeLord;
    private Dictionary<int, PlayerController> _players;
    private bool _isCountingTillGameStart;
    private float _secondsTillGame;

    void Awake()
    {
        int totalFrames = Constants.FrameRate * 60 * 2;
        _timeLord = new TimeLord(totalFrames);

        _players = new Dictionary<int, PlayerController>();
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
        _players.Add(id, pc);
    }

    public void HideAllPlayers()
    {
        foreach (var player in _players)
        {
            player.Value.Hide();
        }
    }

    public void ShowPlayersInReality()
    {
        HashSet<int> ids = _timeLord.GetPlayersInReality();
        foreach (var id in ids)
        {
            if (_players.ContainsKey(id)) _players[id].Show();
        }
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
