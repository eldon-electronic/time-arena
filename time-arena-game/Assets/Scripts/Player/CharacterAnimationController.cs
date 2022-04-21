using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator PlayerAnim;
    [SerializeField] private PlayerGrab _grab;
    [SerializeField] private  PhotonView _view;
    

    // Update is called once per frame
    void Update()
    {
        if(!_view.IsMine) return;
        AnimationKeyControl();
        
        
    }
    public void AnimationKeyControl(){
        
        if(Input.GetKeyDown(KeyCode.W)){
            StartRunningForwards();
            Debug.Log(PlayerAnim);
        } 
        if(Input.GetKeyDown(KeyCode.S)) StartRunningBackwards();
        if(Input.GetKeyUp(KeyCode.W)) StopRunningForwards();
        if(Input.GetKeyUp(KeyCode.S)) StopRunningBackwards();
        if(Input.GetKeyDown(KeyCode.Space)){
            //PlayerAnim.Play("Jumping");
            StartJumping();
            Debug.Log("isJumping");
        }
        if(Input.GetKeyUp(KeyCode.Space)){
            //PlayerAnim.Play("Jumping");
            StopJumping();
            //Debug.Log("isJumping");
        }
        if(Input.GetMouseButtonDown(0)) _grab.Grab();
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
