using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviour{

	//variables referring to the game state
	public float gameLength = 5f * 60f; //5 minute rounds * sixty seconds
	public float timeElapsedInGame = 0f;

	public PlayerMovement player;

	public bool gameStarted = false;
	public bool gameEnded = false;

	public int winningTeam = 1;

	public Vector3[] hiderSpawnPoints = {new Vector3(4f, -0.5f, 0.3f), new Vector3(7f, -0.5f, 3f), new Vector3(7f, -0.5f, -3f), new Vector3(9f, -0.5f, 0.3f), new Vector3(13f, 1.2f, 0.3f)};
	public Vector3 seekerSpawnPoint = new Vector3(28f, -0.5f, 0.3f);

	// Start is called before the first frame update
	void Start() {
		//prevent anyone else from joining room
		PhotonNetwork.CurrentRoom.IsOpen = false;

		GameObject[] clients = GameObject.FindGameObjectsWithTag("Client");
		if(clients.Length == 1){
			player = clients[0].GetComponent<PlayerMovement>();
			player.game = this;
			if(PhotonNetwork.IsMasterClient){
				setupNewGame(player);
			}
		} else {
			Debug.Log("wtf");
		}
	}

	//initialise teams and spawn locations for the new game;
	void setupNewGame(PlayerMovement client){
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		List<PlayerMovement> players = new List<PlayerMovement>();
		players.Add(client);
		for(int i = 0; i < objs.Length; i++){
			players.Add(objs[i].GetComponent<PlayerMovement>());
		}
		//set players position to spawn point
		Debug.Log(players.Count);
		if(players.Count > 1){	//if testing with one player, they are hider, otherwise one player will randomly be seeker
			players[Random.Range(0, players.Count-1)].changeTeam();
		}
		int n = 0;
		for(int i = 0; i < players.Count; i++){
			if( players[i].team == 1 ){
				players[i].movePlayer(hiderSpawnPoints[n++], new Vector3(0f, -90f, 0f));
			} else {
				players[i].movePlayer(seekerSpawnPoint, new Vector3(0f, 90f, 0f));
			}
		}
	}

	// Update is called once per frame
	void Update() {
		//increment timer
		if(!gameEnded){
			timeElapsedInGame += Time.deltaTime;
		}
		//if pregame timer is counting // else game is in play
		if(!gameStarted){
			if(timeElapsedInGame >= 5f){
				gameStarted = true;
				timeElapsedInGame = 0f;
			}
		} else {
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
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		List<PlayerMovement> players = new List<PlayerMovement>();
		players.Add(player);
		for(int i = 0; i < objs.Length; i++){
			players.Add(objs[i].GetComponent<PlayerMovement>());
		}
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
