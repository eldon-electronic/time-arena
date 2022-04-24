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
	private bool _damageWindow = false;
  private SceneController sceneController;

  void Start(){
    sceneController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<SceneController>();
  }

    void Update()
    {
      // If grabbing, check for intersection with player.
  		if (_damageWindow)
  		{
  			Collider[] playersGrab = Physics.OverlapSphere(collider.gameObject.transform.position, collider.radius, GrabMask);
  			foreach (var playerGotGrab in playersGrab)
  			{
  				// Call grabplayer function on that player.
  				PlayerController targetPlayer = playerGotGrab.GetComponent<PlayerController>();
  				if (_player.Team == Constants.Team.Guardian &&
  					targetPlayer.Team == Constants.Team.Miner)
  				{
  					// TODO: grab the miner.
  					Debug.Log("Grabbed a miner");
            PhotonView viewOfMiner = targetPlayer.GetComponent<PhotonView>();
            viewOfMiner?.RPC("RPC_getGrabbed", RpcTarget.All);
  				}
  			}
      }
    }

    [PunRPC]
    void RPC_getGrabbed(){
      if(_view.IsMine){
        sceneController.DecrementPlayerScore();
      } else {
        sceneController.DecrementMinerScore();
      }
    }

    public void Grab()
	{
      StartGrabbing();
	}

    // Function called by animation to enable player to grab others.
	public void StartGrabbing()
	{
		_damageWindow = true;
		PlayerAnim.SetBool("isGrabbing", true);

	}

	// Function called by animation to disable player to grab others.
	public void StopGrabbing()
	{
		_damageWindow = false;
		PlayerAnim.SetBool("isGrabbing", false);
	}
}
