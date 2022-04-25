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
	public bool damageWindow = false;
  private SceneController _sceneController;

    void Start(){
      _sceneController = FindObjectOfType<PreGameController>();
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
  		if (damageWindow)
  		{
        Grab();
      }
    }

    [PunRPC]
    void RPC_getGrabbed(){
      if(_view.IsMine){
        _sceneController.DecrementPlayerScore();
      } else {
        _sceneController.DecrementMinerScore();
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
          continue;
        }
      }
    }
  }

  private void SetGame(GameController game) { _sceneController = game; }

}
