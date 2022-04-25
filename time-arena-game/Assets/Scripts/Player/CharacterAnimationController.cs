using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator PlayerAnim;
    [SerializeField] private PlayerGrab _grab;
    [SerializeField] private  PhotonView _view;
    private bool _paused = false;
    private float grabTimer = 0;
    private bool mouseDown = false;
    private bool mouseReset = true;

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
      if(mouseDown && mouseReset){
        mouseReset = false;
        StartGrabbing();
      } else if(_grab._damageWindow) {
        grabTimer += Time.deltaTime;
        if(grabTimer >= 1){
          StopGrabbing();
          mouseReset = true;
        }
      }

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
        if(Input.GetMouseButtonDown(0)) mouseDown = true;
        if(Input.GetMouseButtonUp(0)) mouseDown = false;
    }
    public void StartGrabbing(){
      Debug.Log("started grab");
      _grab._damageWindow = true;
      PlayerAnim.SetBool("isGrabbing", true);
    }
    public void StopGrabbing(){
      Debug.Log("stopped grab");
        _grab._damageWindow = false;
        PlayerAnim.SetBool("isGrabbing", false);
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
}
