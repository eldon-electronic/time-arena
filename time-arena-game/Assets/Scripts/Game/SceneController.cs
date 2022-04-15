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
	public static event Action<int> scoreChange;

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
		scoreChange?.Invoke(_minerScore);
	}
}
