using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject PlayerMinerPrefab;
    public GameObject PlayerGuardianPrefab;
	public Vector3[] SpawningPoint;

    void Awake()
    {
        SpawningPoint = new Vector3[]
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
        int n = (int) (SpawningPoint.Length * Random.value);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(PlayerGuardianPrefab.name, SpawningPoint[n], Quaternion.identity);    
        }
        else PhotonNetwork.Instantiate(PlayerMinerPrefab.name, SpawningPoint[n], Quaternion.identity);
    }
}
