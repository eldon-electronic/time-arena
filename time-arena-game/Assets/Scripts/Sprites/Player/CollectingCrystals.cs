using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


// This script must be attached to a player object with either a CharacterController or both a
// Collider and RigidBody in order for it to be able to detect collisions.
public class CollectingCrystals : MonoBehaviour
{
  [SerializeField] private PhotonView _view;
  [SerializeField] private PlayerMinerController _player;

  private void Awake()
  {
    if (!_view.IsMine) Destroy(this);
  }

  public void OnTriggerEnter(Collider col)
  {
    if (col.gameObject.tag == "Collectable")
    {
      PhotonView viewOfCrystal = col.gameObject.GetComponent<PhotonView>();
      viewOfCrystal.RPC("RPC_Collect", RpcTarget.All);
      _player.IncrementedScore();
    }
  }
}
