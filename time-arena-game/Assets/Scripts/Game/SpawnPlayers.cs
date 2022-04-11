using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{

    // Prefab defining a player object.
    public GameObject playerPrefab;

	// Spawn point array.
	public Vector3[] spawningPoint = {
		new Vector3(-62f, -2f, 20f),
		new Vector3(2f, 5f, 0f),
		new Vector3(15f, 5f, 0f),
		new Vector3(23f, 5f, 0f),
		new Vector3(30f, 5f, 0f)
	};

    private TimeLord _timeLord;

    void Start()
    {
        // Spawn a new player into the scene.
        int n = (int)((spawningPoint.Length) * Random.value);
        PhotonNetwork.Instantiate(playerPrefab.name, spawningPoint[0], Quaternion.identity);
    }
}
