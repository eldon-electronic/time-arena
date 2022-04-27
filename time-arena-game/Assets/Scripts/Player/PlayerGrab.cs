using System;
using UnityEngine;
using Photon.Pun;

public class PlayerGrab : MonoBehaviour
{
	[SerializeField] private LayerMask _grabMask;
  [SerializeField] private SphereCollider _collider;
  [SerializeField] private PlayerController _player;
  private bool _grabCooldown;

  void Awake()
  {
    if (_player.Team == Constants.Team.Miner)
    {
      Destroy(this);
      return;
    }
    _grabCooldown = false;
  }

  public void Grab()
	{
    Collider[] grabbedPlayers = Physics.OverlapSphere(transform.position, _collider.radius, _grabMask);
    foreach (var player in grabbedPlayers)
    {
      PlayerController playerController = player.gameObject.GetComponent<PlayerController>();
      if (playerController.Team == Constants.Team.Miner)
      {
        PhotonView view = playerController.gameObject.GetComponent<PhotonView>();
        view.RPC("RPC_getGrabbed", RpcTarget.All);
      }
    }
  }
}
