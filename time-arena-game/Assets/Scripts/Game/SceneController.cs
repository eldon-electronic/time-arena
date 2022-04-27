using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class SceneController: MonoBehaviour
{
    protected Dictionary<int, PlayerController> _miners;
	protected Dictionary<int, PlayerController> _guardians;
	protected TimeLord _timeLord;
  	protected int _minerScore;
  	protected int _clientScore;
    public static event Action<int, int> scoreChange;

	protected void CreateTimeLord(bool logging=false, bool diagnostics=false)
    {
        if (logging && PhotonNetwork.CurrentRoom.PlayerCount != 1)
        {
            throw new InvalidOperationException("Logging is only allowed in single player tests.");
        }
        else if (diagnostics && PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            throw new InvalidOperationException("Diagnostics is only allowed in multiplayer tests.");
        }
        else
        {
			// Beware that if running diagnostics, non-master clients must be run in the Unity Editor.
            int totalFrames = Constants.PreGameLength * Constants.FrameRate;
            _timeLord = new ProxyTimeLord(totalFrames, logging, diagnostics && !(PhotonNetwork.IsMasterClient));
        }
    }

	public void Register(PlayerController pc)
    {
		if (pc.Team == Constants.Team.Guardian) _guardians.Add(pc.ID, pc);
		else _miners.Add(pc.ID, pc);
    }

    public Constants.Team GetTeam(int playerID)
	{
		if (_miners.ContainsKey(playerID)) return Constants.Team.Miner;
		else if (_guardians.ContainsKey(playerID)) return Constants.Team.Guardian;
		else throw new KeyNotFoundException("No team associated with the given playerID.");
	}

    public void HideAllPlayers()
	{
		foreach (var guardian in _guardians)
		{
			guardian.Value.Hide();
		}
		foreach (var miner in _miners)
		{
			miner.Value.Hide();
		}
	}

    public void ShowPlayersInReality()
	{
		HashSet<int> playerIDs = _timeLord.GetPlayersInReality();
		foreach (var id in playerIDs)
		{
			if (_guardians.ContainsKey(id)) _guardians[id].Show();
			else if (_miners.ContainsKey(id)) _miners[id].Show();
		}
	}

    public void IncrementMinerScore()
  {
    _minerScore++;
    scoreChange?.Invoke(_clientScore, _minerScore);
  }

    public void IncrementPlayerScore()
  {
    _minerScore++;
    _clientScore++;
    scoreChange?.Invoke(_clientScore, _minerScore);
  }

	public TimeLord GetTimeLord() { return _timeLord;}
}
