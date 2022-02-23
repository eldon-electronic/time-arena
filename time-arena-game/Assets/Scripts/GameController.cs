using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviour
{

    // variables referring to the game state
    public float gameLength = 5f * 60f; //5 minute rounds * sixty seconds
    public float timeElapsedInGame = 0f;

	public PlayerMovement player;
	public List<PlayerMovement> players;

	// list to keep track of elapsed time for all players
  	public Dictionary<int, float> otherPlayersElapsedTime = new Dictionary<int, float>();

	public bool gameStarted = false;
	public bool gameEnded = false;

	public int winningTeam = 1;

	public Vector3[] hiderSpawnPoints = {new Vector3(-42f, 0f, 22f), new Vector3(-15f, -0.5f, -4f), new Vector3(-12f, -0.5f, -40f), new Vector3(-47f, -0.5f, -8f), new Vector3(-36f, -2.5f, 2.2f)};
	public Vector3 seekerSpawnPoint = new Vector3(-36f, -2f, -29f);

	// Start is called before the first frame update
	void Start() {
		// prevent anyone else from joining room
		PhotonNetwork.CurrentRoom.IsOpen = false;

		GameObject[] clients = GameObject.FindGameObjectsWithTag("Client");
		if (clients.Length == 1) {
			player = clients[0].GetComponent<PlayerMovement>();
			player.game = this;
			if(PhotonNetwork.IsMasterClient){
				setupNewGame(player);
			}
		} else {
			Debug.Log("wtf");
		}

		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		players.Add(player); //player.timeTravel.GetTimePosition()
		otherPlayersElapsedTime.Add(player.view.ViewID, 0); // adds yourself before adding remaining players
		for(int i = 0; i < objs.Length; i++){
			players.Add(objs[i].GetComponent<PlayerMovement>());
			otherPlayersElapsedTime.Add(players[i].view.ViewID, 0);
		}
	}

    // initialise teams and spawn locations for the new game;
	void setupNewGame(PlayerMovement client){
		// set players position to spawn point
		if (players.Count > 1) { // if testing with one player, they are hider, otherwise one player will randomly be seeker
			players[Random.Range(0, players.Count-1)].changeTeam();
		}
		int n = 0;
		for (int i = 0; i < players.Count; i++) {
			if (players[i].team == 1) {
				players[i].movePlayer(hiderSpawnPoints[n++], new Vector3(0f, -90f, 0f));
			} else {
				players[i].movePlayer(seekerSpawnPoint, new Vector3(0f, 90f, 0f));
			}
		}
	}

	// Update is called once per frame
	void Update() {
		// increment global timer and individual player timers
		if (!gameEnded) {
			timeElapsedInGame += Time.deltaTime;
			List<int> keys = new List<int>(otherPlayersElapsedTime.Keys);
			foreach(int key in keys){
				otherPlayersElapsedTime[key] += Time.deltaTime / gameLength;
			}
		}
		if (!gameStarted) { // if pregame timer is counting
			if (timeElapsedInGame >= 5f) {
				gameStarted = true;
				timeElapsedInGame = 0f;
				List<int> keys = new List<int>(otherPlayersElapsedTime.Keys);
				foreach(int key in keys){
					otherPlayersElapsedTime[key] = 0f;
				}
			}
		} else { // else game is in play
			if(timeElapsedInGame >= gameLength && !gameEnded){
				gameEnded = true;
				winningTeam = 1;
				player.onGameEnded();
			}
		}
		checkGameOver();
	}

	// checks to see if there are no hiders left
	public void checkGameOver(){
		bool isHidersRemaining = true;
		for(int i = 0; i < players.Count; i++){
			isHidersRemaining &= (players[i].team == 0);
		}
		if(!isHidersRemaining){
			gameEnded = true;
			winningTeam = 0;
			player.onGameEnded();
		}
	}
}
