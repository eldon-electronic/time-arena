using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviour
{

    // Variables referring to the game state.
	// 5 minute rounds * sixty seconds.
    public float GameLength = 5f * 60f;
    public float TimeElapsedInGame = 0f;

	private PlayerController _client;
	public List<PlayerController> _players;

	// List to keep track of elapsed time for all players.
  	public Dictionary<int, float> OtherPlayersElapsedTime = new Dictionary<int, float>();

	public bool GameStarted = false;
	public bool GameEnded = false;
	public Constants.Team WinningTeam = Constants.Team.Miner;


	void Start() {
		// Prevent anyone else from joining room.
		PhotonNetwork.CurrentRoom.IsOpen = false;

		AddClient();
		AddPlayers();
		
		if (PhotonNetwork.IsMasterClient) SetupNewGame();
	}


	// ------------ START HELPER FUNCTIONS ------------

	private void AddClient()
	{
		GameObject[] clients = GameObject.FindGameObjectsWithTag("Client");
		if (clients.Length == 1)
		{
			_client = clients[0].GetComponent<PlayerController>();
			_players.Add(_client);
			OtherPlayersElapsedTime.Add(_client.View.ViewID, 0f);
			_client.Game = this;
		}
		else Debug.LogError("Number of clients is not 1");
	}

	private void AddPlayers()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in players)
		{
			PlayerController playerComponent = player.GetComponent<PlayerController>();
			_players.Add(playerComponent);
			OtherPlayersElapsedTime.Add(playerComponent.View.ViewID, 0f);
		}
	}

    // Initialise teams and spawn locations for the new game.
	private void SetupNewGame()
	{
		// If testing with one player, they are hider, otherwise one player will randomly be seeker.
		if (_players.Count > 1)
		{
			int randomIndex = Random.Range(0, _players.Count - 1); 
			_players[randomIndex].GetFound();
		}
	}


	// ------------ UPDATE METHODS ------------

	void Update() {
		// Increment global timer and individual player timers.
		if (!GameEnded)
		{
			TimeElapsedInGame += Time.deltaTime;
			List<int> keys = new List<int>(OtherPlayersElapsedTime.Keys);
			foreach (int key in keys)
			{
				OtherPlayersElapsedTime[key] += Time.deltaTime / GameLength;
			}
		}

		// If pregame timer is counting.
		if (!GameStarted)
		{
			if (TimeElapsedInGame >= 5f)
			{
				GameStarted = true;
				TimeElapsedInGame = 0f;
				List<int> keys = new List<int>(OtherPlayersElapsedTime.Keys);
				foreach (int key in keys)
				{
					OtherPlayersElapsedTime[key] = 0f;
				}
			}
		}
		// Else game is in play.
		else
		{
			CheckHidersLeft();
			if (TimeElapsedInGame >= GameLength && !GameEnded)
			{
				GameEnded = true;
				WinningTeam = Constants.Team.Miner;
				_client.OnGameEnded();
			}
		}
	}

	// Checks to see if there are no hiders left.
	public void CheckHidersLeft()
	{
		bool isHidersRemaining = false;
		for (int i = 0; i < _players.Count; i++)
		{
			isHidersRemaining |= (_players[i].Team == Constants.Team.Miner);
		}
		if (!isHidersRemaining)
		{
			// Code reaches here even though hiders are remaining.
			GameEnded = true;
			WinningTeam = Constants.Team.Guardian;
			_client.OnGameEnded();
		}
	}
}
