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

	public Vector3[] spawnPosTeam1 = {new Vector3(4f, -0.5f, 0.3f), new Vector3(7f, -0.5f, 3f), new Vector3(7f, -0.5f, -3f), new Vector3(9f, -0.5f, 0.3f), new Vector3(13f, 1.2f, 0.3f)};
	public Vector3[] spawnPosTeam2 = {new Vector3(28f, -0.5f, 0.3f), new Vector3(26f, -0.5f, 3f), new Vector3(26f, -0.5f, -3f), new Vector3(23f, -0.5f, 0.3f), new Vector3(29f, 1.2f, 0.3f)};

	// Start is called before the first frame update
	void Start() {
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
		//shuffle List
		int n = players.Count;
		while (n > 1) {
			n--;
			int k = Random.Range(0, n + 1);
			PlayerMovement value = players[k];
			players[k] = players[n];
			players[n] = value;
		}

		//iterate over list and set every other one to opposite teams (weighted towards hiders)
		for(int i = 0; i < players.Count; i++){
			if( (i & 1) == 1){
				players[i].changeTeam();
				players[i].movePlayer(spawnPosTeam1[(int)(i/2)], new Vector3(0f, 90f, 0f));
			} else {
				players[i].movePlayer(spawnPosTeam2[(int)(i/2)], new Vector3(0f, -90f, 0f));

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
				player.onGameEnded();
			}
		}
	}
}
