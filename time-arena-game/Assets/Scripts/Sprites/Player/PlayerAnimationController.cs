using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
  [SerializeField] private CharacterAnimation _animation;
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
    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
    {
      _animation.SetRunningForwards(true);
    }
    if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
    {
      _animation.SetRunningForwards(false);
    }
    if (Input.GetKeyDown(KeyCode.S)) _animation.SetRunningBackwards(true);
    if (Input.GetKeyUp(KeyCode.S)) _animation.SetRunningBackwards(false);
    if (Input.GetKeyDown(KeyCode.Space)) _animation.SetJumping(true);
    if (Input.GetKeyUp(KeyCode.Space)) _animation.SetJumping(false);
    if (Input.GetMouseButtonDown(0) && !_grabCooldown && _player.Team == Constants.Team.Guardian)
    {
      _animation.SetGrabbing(true);
    }
  }

  public void SetGrabCooldown(bool value)
  {
    _grabCooldown = value;
  }
}
