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
        Quaternion playerRotation = _channels.GetChild(channelID).Find("PlayerSpawnPoint").rotation;
        Vector3 objectiveSpawnPoint = _channels.GetChild(channelID).Find("ObjectiveSpawnPoint").position;
        Quaternion objectiveRotation = _channels.GetChild(channelID).Find("ObjectiveSpawnPoint").rotation;
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

        object[] data = new object[] { channelID };
        PhotonNetwork.Instantiate(playerPrefab, playerSpawnPoint, playerRotation, 0, data);
        PhotonNetwork.Instantiate(objectivePrefab, objectiveSpawnPoint, objectiveRotation);
    }
}
