using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollectingCrystals : MonoBehaviour
{
  [SerializeField] private PlayerController _player;
  [SerializeField] private PhotonView _view;

  public void Awake()
  {
    if (_player.Team == Constants.Team.Guardian) Destroy(this);
  }

  public void OnTriggerEnter(Collider col)
  {
    Debug.Log("Collision detected");
    if (col.gameObject.tag == "Collectable")
    {
      PhotonView viewOfCrystal = col.gameObject.GetComponent<PhotonView>();
      if (viewOfCrystal.IsMine)
      {
        viewOfCrystal.RPC("RPC_Collect", RpcTarget.All);
        _view.RPC("RPC_incrementScore", RpcTarget.All);
      }
    }
  }
}
