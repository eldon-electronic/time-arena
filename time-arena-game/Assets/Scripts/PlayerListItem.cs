using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;
using Photon.Pun;

public class PlayerListItem : MonoBehaviourPunCallbacks
{

    [SerializeField] TMP_Text _playerName;
    private Player _player;

    public void SetUp(Player player) { 
        _player = player;
        _playerName.text = player.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        if (_player ==  otherPlayer) {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom() {
        Destroy(gameObject);
    }
}
