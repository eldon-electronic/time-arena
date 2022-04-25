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
    }

    public void OnEnable()
    {
        GameController.gameActive += SetGame;
    }

    public void OnDisable()
    {
        GameController.gameActive -= SetGame;
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
      Debug.Log("A miner has been grabbed");
      //TODO: respawn?
    }

    public void Grab()
	{
    Debug.Log("checking for grab target");
    Collider[] playersGrab = Physics.OverlapSphere(collider.gameObject.transform.position, collider.radius, GrabMask);
    foreach (var playerGotGrab in playersGrab)
    {
      Debug.Log("Checking target" + playerGotGrab);
      // Call grabplayer function on that player.
      PlayerController targetPlayer = playerGotGrab.gameObject.GetComponent<PlayerController>();
      if(targetPlayer!=null){
        Debug.Log("found PC");
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

  private void SetGame(GameController game) { sceneController = game; }

}
