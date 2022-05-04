using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField] private PhotonView _view;
    [SerializeField] private Transform _channels;
    [SerializeField] private GameObject _playerMinerPrefab;
    [SerializeField] private GameObject _playerGuardianPrefab;
    [SerializeField] private GameObject _collectablePrefab;
    [SerializeField] private GameObject _npcPrefab;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int channelID = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                 _view.RPC("RPC_setChannel", player, channelID);
                 channelID++;
            }
        }
    }

    [PunRPC]
    private void RPC_setChannel(int channelID)
    {
        Vector3 playerSpawnPoint = _channels.GetChild(channelID).Find("PlayerSpawnPoint").position;
        Vector3 objectiveSpawnPoint = _channels.GetChild(channelID).Find("ObjectiveSpawnPoint").position;
        string playerPrefab;
        string objectivePrefab;

        if (PlayerPrefs.GetString("team") == "guardian")
        {
            playerPrefab = _playerGuardianPrefab.name;
            objectivePrefab = _npcPrefab.name;
        }
        else
        {
            playerPrefab = _playerMinerPrefab.name;
            objectivePrefab = _collectablePrefab.name;
        }

        PhotonNetwork.Instantiate(playerPrefab, playerSpawnPoint, Quaternion.identity);
        PhotonNetwork.Instantiate(objectivePrefab, objectiveSpawnPoint, Quaternion.identity);
    }
}
