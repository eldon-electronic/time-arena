using System;
using UnityEngine;
using Photon.Pun;

public class PlayerGrab : MonoBehaviour
{
    [SerializeField] private Animator PlayerAnim;
	[SerializeField] private LayerMask GrabMask;
  [SerializeField] private PlayerController _player;
  [SerializeField] private SphereCollider collider;
  private PhotonView _view;
	public bool _damageWindow = false;
  private SceneController sceneController;

    void Start(){
      sceneController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<SceneController>();
    }

    void Update()
    {
      // If grabbing, check for intersection with player.
  		if (_damageWindow)
  		{
        Grab();
      }
    }

    [PunRPC]
    void RPC_getGrabbed(){
      if(_view.IsMine){
        sceneController.DecrementPlayerScore();
      } else {
        sceneController.DecrementMinerScore();
      }
      //TODO: respawn?
    }

    public void Grab()
	{
    Debug.Log("checking for grab target");
    Collider[] playersGrab = Physics.OverlapSphere(collider.gameObject.transform.position, collider.radius, GrabMask);
    foreach (var playerGotGrab in playersGrab)
    {
      // Call grabplayer function on that player.
      PlayerController targetPlayer = playerGotGrab.GetComponent<PlayerController>();
      if(targetPlayer!=null){
        if (_player.Team == Constants.Team.Guardian && targetPlayer.Team == Constants.Team.Miner)
        {
          Debug.Log("Grabbed a miner");
          PhotonView viewOfMiner = targetPlayer.GetComponent<PhotonView>();
          viewOfMiner?.RPC("RPC_getGrabbed", RpcTarget.All);
          _damageWindow = false;
          continue;
        }
      }
    }
  }
}
