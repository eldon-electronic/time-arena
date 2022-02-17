using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviour{

	//variables referring to the game state
	public float gameLength = 5f * 60f; //5 minute rounds * sixty seconds
	public float timeElapsedInGame = 0f;

	public bool gameStarted = false;
	public bool gameEnded = false;

	public Vector3[] spawnPosTeam1 = {new Vector3(0, 0, 0), new Vector3(0, 0, 2), new Vector3(0, 0, 4), new Vector3(0, 0, 6), new Vector3(0, 0, 8)};
	public Vector3[] spawnPosTeam2 = {new Vector3(0, 0, 0), new Vector3(0, 0, 2), new Vector3(0, 0, 4), new Vector3(0, 0, 6), new Vector3(0, 0, 8)};

	// Start is called before the first frame update
	void Start() {
		GameObject[] clients = GameObject.FindGameObjectsWithTag("Client");
		if(clients.Length == 1){
			PlayerMovement player = clients[0].GetComponent<PlayerMovement>();
			player.game = this;
			if(PhotonNetwork.IsMasterClient){
				setupNewGame(player);
			}
		} else {
			Debug.Log("wtf");
		}
	}

	void setupNewGame(PlayerMovement client){
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		List<PlayerMovement> players = new List<PlayerMovement>();
		players.Add(client);
		for(int i = 0; i < objs.Length; i++){
			players.Add(objs[i].GetComponent<PlayerMovement>());
		}
		//set players position to spawn point
	}

	// Update is called once per frame
	void Update() {
		//increment timer
		timeElapsedInGame += Time.deltaTime;
		//if pregame timer is counting // else game is in play
		if(!gameStarted){
			if(timeElapsedInGame >= 5f){
				gameStarted = true;
				timeElapsedInGame = 0f;
			}
		} else {
			if(timeElapsedInGame >= gameLength){
				gameEnded = true;
			}
		}
	}
}
