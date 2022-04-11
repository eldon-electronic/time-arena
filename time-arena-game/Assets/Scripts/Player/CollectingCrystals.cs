using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollectingCrystals : MonoBehaviour
{
  [SerializeField] private PlayerController _player;
    private GameController _game;


    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Collectable" && _player.Team == Constants.Team.Miner && col.gameObject.layer == 3)
        {
            if (_game != null) _player.incrScore();
            PhotonNetwork.Destroy(col.gameObject);
        }
    }

    public void SetGame(GameController game) { _game = game; }
}
