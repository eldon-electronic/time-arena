using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour{

	//prefab defining a player object
	public GameObject playerPrefab;

	//spawn point array
	public Vector3[] spawnPoint = {
		new Vector3(0, 0, 0),
		new Vector3(0, 0, 0),
		new Vector3(0, 0, 0),
		new Vector3(0, 0, 0),
		new Vector3(0, 0, 0),
	};

	// Start is called before the first frame update
	void Start(){
		//spawn a new player into the scene
		//int n = (int) ((spawnPoint.Length-1) * Random.value);

		PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
	}
}
