using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollectingCrystals : MonoBehaviour
{
  [SerializeField] private PhotonView _view;

  public void OnTriggerEnter(Collider col)
  {
    if (col.gameObject.tag == "Collectable")
    {
      Debug.Log("Collided with crystal");
      PhotonView viewOfCrystal = col.gameObject.GetComponent<PhotonView>();
      if (viewOfCrystal.IsMine)
      {
        viewOfCrystal.RPC("RPC_Collect", RpcTarget.All);
        _view.RPC("RPC_incrementScore", RpcTarget.All);
        Debug.Log("Called RPC functions");
      }
    }
  }
}
