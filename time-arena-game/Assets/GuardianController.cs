using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianController : MonoBehaviour
{
    public Animator PlayerAnim;
   // private bool _isAnimating;
    // Start is called before the first frame update
    void Start(){

      
    }
    void Update() {

       
        
    }

    public void GuardianGrabbing(){

        PlayerAnim.SetBool("isGrabbing", true);
        //_isIdle = false;
        //_isAnimating = true;
        //GuardianAnim.SetBool("isGuardianIdle", false);
    }
    public void GuardianJumping(){

        PlayerAnim.SetBool("isJumping", true);
        //_isAnimating = true;
        //_isIdle = false;
        
    }
    public void GuardianRunning(){

        PlayerAnim.SetBool("isRunning", true);
        //_isIdle = false;
    }
	public void StopGuardianGrabbing(){

        //if(_isAnimating == true){

            PlayerAnim.SetBool("isGrabbing", false);
            //_isAnimating = false;

        //}
    }
	 public void StopGuardianJumping(){

         //if(_isAnimating == true){

            PlayerAnim.SetBool("isJumping", false);
           // _isAnimating = false;

       // }
  
    }
	 public void StopGuardianRunning(){

        PlayerAnim.SetBool("isRunning", false);
        //_isIdle = false;
    }
}
