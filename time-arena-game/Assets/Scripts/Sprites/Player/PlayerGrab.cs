using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerGrab : MonoBehaviour
{
	[SerializeField] private LayerMask _grabMask;
  [SerializeField] private SphereCollider _collider;
  [SerializeField] private PlayerController _player;
  [SerializeField] private PlayerAnimationController _animation;
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

  void Update()
  {
    if (!_grabCooldown && Input.GetMouseButtonDown(0))
    {
      Grab();
      _grabCooldown = true;
      _animation.SetGrabCooldown(_grabCooldown);
      StartCoroutine(GrabReset(3));
    }
  }

  //enumerator coroutine to be called when grabbing
  private IEnumerator GrabReset(int grabDelay)
  {
    yield return new WaitForSeconds(grabDelay);
    _grabCooldown = false;
    _animation.SetGrabCooldown(_grabCooldown);
  }

  public void Grab()
	{
    Collider[] grabbedPlayers = Physics.OverlapSphere(transform.position, _collider.radius, _grabMask);
    foreach (var player in grabbedPlayers)
    {
      PlayerController playerController = player.transform.root.GetComponent<PlayerController>();
      if (playerController.Team == Constants.Team.Miner)
      {
        PhotonView view = playerController.gameObject.GetComponent<PhotonView>();
        view.RPC("RPC_getGrabbed", RpcTarget.All);
        _player.IncrementScore();
      }
    }
  }
}
