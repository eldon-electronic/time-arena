using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour{

	//prefab defining a player object
	public GameObject playerPrefab;

	//spawn point array
	public Vector3[] spawningPoint = {
		new Vector3(7f, 5f, 0f),
		new Vector3(0f, 5f, 0f),
		new Vector3(15f, 5f, 0f),
		new Vector3(23f, 5f, 0f),
		new Vector3(30f, 5f, 0f)
	};

	// Start is called before the first frame update
	void Start(){
		//spawn a new player into the scene
		int n = (int) ((spawningPoint.Length) * Random.value);
		PhotonNetwork.Instantiate(playerPrefab.name, spawningPoint[n], Quaternion.identity);
	}
}
