using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
  [SerializeField] protected Animator _playerAnim;

  public void SetRunningForwards(bool value)
  {
    _playerAnim.SetBool("isRunningForwards", value);
  }

  public void SetRunningBackwards(bool value)
  {
    _playerAnim.SetBool("isRunningBackwards", value);
  }

  public void SetJumping(bool value)
  {
    _playerAnim.SetBool("isJumping", value);
  }

  public void SetGrabbing(bool value)
  {
    _playerAnim.SetBool("isGrabbing", value);
  }

  public void StopGrabbing()
  {
    _playerAnim.SetBool("isGrabbing", false);
  }
}
