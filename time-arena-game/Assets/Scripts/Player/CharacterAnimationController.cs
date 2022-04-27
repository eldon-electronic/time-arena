using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
  [SerializeField] private Animator PlayerAnim;
  [SerializeField] private PlayerGrab _grabber;
  [SerializeField] private  PhotonView _view;
  [SerializeField] private  PlayerController _player;
  private bool _paused;
  private bool _grabCooldown;

  void Awake()
  {
      if (!_view.IsMine)
      {
          Destroy(this);
          return;
      }
      _paused = false;
      _grabCooldown = false;
  }

  void OnEnable()
  {
    PauseManager.paused += updatePause;
  }

  void OnDisable()
  {
    PauseManager.paused -= updatePause;
  }

  void updatePause(bool newVal) { _paused = newVal; }

  void Update()
  {
    if (_paused) return;
    if (Input.GetKeyDown(KeyCode.W)) StartRunningForwards();
    if (Input.GetKeyDown(KeyCode.S)) StartRunningBackwards();
    if (Input.GetKeyUp(KeyCode.W)) StopRunningForwards();
    if (Input.GetKeyDown(KeyCode.A)) StartRunningForwards();
    if (Input.GetKeyUp(KeyCode.A)) StopRunningForwards();
    if (Input.GetKeyDown(KeyCode.D)) StartRunningForwards();
    if (Input.GetKeyUp(KeyCode.D)) StopRunningForwards();
    if (Input.GetKeyUp(KeyCode.S)) StopRunningBackwards();
    if (Input.GetKeyDown(KeyCode.Space)) StartJumping();
    if (Input.GetKeyUp(KeyCode.Space)) StopJumping();
    if (Input.GetMouseButtonDown(0)) StartGrabbing();
    if (Input.GetMouseButtonUp(0)) StopGrabbing();
  }

  public void StartGrabbing()
  {
    if (!_grabCooldown && _player.Team == Constants.Team.Guardian && _grabber != null)
    {
      PlayerAnim.SetBool("isGrabbing", true);
      _grabCooldown = true;
      _grabber.Grab();
      GrabReset(3);
    }
  }

  public void StopGrabbing()
  {
    PlayerAnim.SetBool("isGrabbing", false);
  }

  //enumerator coroutine to be called when grabbing
  private IEnumerator GrabReset(int grabDelay)
  {
    yield return new WaitForSeconds(grabDelay);
    _grabCooldown = false;
  }

  public void StartRunningForwards()
  {
    PlayerAnim.SetBool("isRunningForwards",true);
  }

  public void StartRunningBackwards()
  {
    PlayerAnim.SetBool("isRunningBackwards",true);
  }

  public void StopRunningForwards()
  {
    PlayerAnim.SetBool("isRunningForwards",false);
  }

  public void StopRunningBackwards()
  {
    PlayerAnim.SetBool("isRunningBackwards",false);
  }

  public void StartJumping()
  {
    PlayerAnim.SetBool("isJumping",true);
  }

  public void StopJumping()
  {
    PlayerAnim.SetBool("isJumping",false);
  }
}
