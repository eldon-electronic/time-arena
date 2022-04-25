using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator PlayerAnim;
    [SerializeField] private PlayerGrab _grab;
    [SerializeField] private  PhotonView _view;
    [SerializeField] private  PlayerController _playerController;
    private bool _paused = false;
    private bool _grabCooldown = false;

    void Awake() {
        PauseManager.paused += updatePause;
        if(!_view.IsMine){
            Destroy(this);
            return;
        }

    }

    void updatePause(bool newVal){
      _paused = newVal;
    }

    // Update is called once per frame
    void Update()
    {

        AnimationKeyControl();

    }
    public void AnimationKeyControl(){

        if(Input.GetKeyDown(KeyCode.W))StartRunningForwards();
        if(Input.GetKeyDown(KeyCode.S)) StartRunningBackwards();
        if(Input.GetKeyUp(KeyCode.W)) StopRunningForwards();
        if(Input.GetKeyDown(KeyCode.A)) StartRunningForwards();
        if(Input.GetKeyUp(KeyCode.A)) StopRunningForwards();
        if(Input.GetKeyDown(KeyCode.D)) StartRunningForwards();
        if(Input.GetKeyUp(KeyCode.D)) StopRunningForwards();
        if(Input.GetKeyUp(KeyCode.S)) StopRunningBackwards();
        if(Input.GetKeyDown(KeyCode.Space))StartJumping();
        if(Input.GetKeyUp(KeyCode.Space))StopJumping();
        if(_playerController.Team == Constants.Team.Guardian){
          Debug.Log("teamcheck pass");
          if(Input.GetMouseButtonDown(0) && !_grabCooldown) {
            Debug.Log("mousepress pass");
            StartGrabbing();
          }
        }
    }
    public void StartGrabbing(){
      PlayerAnim.SetBool("isGrabbing", true);
      _grab.damageWindow = true;
      _grabCooldown = true;
      StartCoroutine(grabReset(3));
    }
    public void StopGrabbing(){
      PlayerAnim.SetBool("isGrabbing", false);
      _grab.damageWindow = false;
    }
    public void StartRunningForwards(){
        PlayerAnim.SetBool("isRunningForwards",true);
    }
    public void StartRunningBackwards(){
        PlayerAnim.SetBool("isRunningBackwards",true);
    }
    public void StopRunningForwards(){
        PlayerAnim.SetBool("isRunningForwards",false);
    }
    public void StopRunningBackwards(){
        PlayerAnim.SetBool("isRunningBackwards",false);
    }
    public void StartJumping(){
        PlayerAnim.SetBool("isJumping",true);
    }
    public void StopJumping(){
        PlayerAnim.SetBool("isJumping",false);
    }

    //enumerator coroutine to be called when grabbing
    public IEnumerator grabReset(int grabDelay){
      yield return new WaitForSeconds(grabDelay);
      _grabCooldown = false;
    }
}
