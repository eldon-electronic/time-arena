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
        if (col.gameObject.tag == "Collectable" && _player.Team == Constants.Team.Miner)
        {
          PhotonView viewOfCrystal = col.gameObject.GetComponent<PhotonView>();
          PhotonView viewOfPlayer = gameObject.GetComponent<PhotonView>();
            if(viewOfCrystal.IsMine && _game != null) {
              viewOfCrystal.RPC("RPC_Collect", RpcTarget.All, viewOfPlayer.ViewID);
              //col.gameObject.GetComponent<CrystalBehaviour>().Collect();
            }
        }
    }

    public void SetGame(GameController game) { _game = game; }
}
