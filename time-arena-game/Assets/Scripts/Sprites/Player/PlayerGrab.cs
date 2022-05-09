using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerGrab : MonoBehaviour, Debuggable
{
	[SerializeField] private LayerMask _grabMask;
  [SerializeField] private SphereCollider _collider;
  [SerializeField] private PlayerController _player;
  [SerializeField] private PlayerAnimationController _animation;
  [SerializeField] private TimeConn _timeConn;
  [SerializeField] private AudioSource _soundSource;
  [SerializeField] private AudioClip _wilhelmScream;
  private bool _grabCooldown;

  void Awake()
  {
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

  void Start()
  {
    FindObjectOfType<HudDebugPanel>().Register(this);
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
        _soundSource.PlayOneShot(_wilhelmScream);
        view.RPC("RPC_getGrabbed", RpcTarget.All);
        _player.IncrementScore();

        // Reset your cooldowns to prevent you from constantly catching miners.
        _timeConn.ResetCooldowns();
        
        // Break out of the loop so we only catch one player once per grab.
        break;
      }
    }
  }

  public Hashtable GetDebugValues()
  {
    Hashtable values = new Hashtable();
    values.Add("grab cooldown", _grabCooldown);
    return values;
  }
}
