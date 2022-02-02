using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour{

	//prefab defining a player object
	public GameObject playerPrefab;

	//spawn point - later should be updated to be a list of possible spawn points
	public Vector3 spawnPoint = new Vector3(0, 0, 0);

	// Start is called before the first frame update
	void Start(){
		//spawn a new player into the scene
		PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity);
	}
}
