using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameController : MonoBehaviour
{
    private TimeLord _timeLord;
    private Dictionary<int, PlayerController> _players;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Constants.FrameRate;

        int totalFrames = Constants.FrameRate * 60 * 2;
        _timeLord = new TimeLord(totalFrames);

        _players = new Dictionary<int, PlayerController>();
    }

    void Update()
    {
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
}
