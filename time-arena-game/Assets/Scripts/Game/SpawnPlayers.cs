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
            new Vector3(65f, 3f, -24f),
            new Vector3(55f, 3f, 45f),
            new Vector3(-20f, 3f, 64f),
            new Vector3(-65f, 3f, -10f),
            new Vector3(1f, 3f, -67f)
        };
    }

    void Start()
    {
        // Spawn a new player into the scene.
        int n = (int) (SpawningPoint.Length * Random.value);

        // TODO: Remove this.
        Vector3 tempSpawnPoint = new Vector3(1, 1, -60);

        if (PlayerPrefs.GetString("team") == "guardian") 
        {
            PhotonNetwork.Instantiate(PlayerGuardianPrefab.name, tempSpawnPoint, Quaternion.identity);    
        }
        else PhotonNetwork.Instantiate(PlayerMinerPrefab.name, tempSpawnPoint, Quaternion.identity);
    }
}
