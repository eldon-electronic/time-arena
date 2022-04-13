using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
	public Vector3[] spawningPoint;
    private TimeLord _timeLord;

    void Awake()
    {
        spawningPoint = new Vector3[]
        {
            new Vector3(-20f, 5f, 0f),
            new Vector3(-10f, 5f, 0f),
            new Vector3(10f, 5f, 0f),
            new Vector3(20f, 5f, 0f),
            new Vector3(30f, 5f, 0f)
        };
    }

    void Start()
    {
        // Spawn a new player into the scene.
        int n = (int)((spawningPoint.Length) * Random.value);
        PhotonNetwork.Instantiate(playerPrefab.name, spawningPoint[0], Quaternion.identity);
    }
}
