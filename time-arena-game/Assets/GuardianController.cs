using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianController : MonoBehaviour
{
    public Animator GuardianAnim;
    // Start is called before the first frame update
    void Start()
    {
        GuardianIdle();
    }

    // Update is called once per frame
    void Update()
    {
        GuardianKeyControl();
        GuardianIdle();

        
    }
    void GuardianKeyControl()
	{
		if (Input.GetKey(KeyCode.W)) GuardianWalking();

		if (Input.GetKeyDown(KeyCode.Space)) GuardianJumping();

		if (Input.GetMouseButtonDown(0))  GuardianGrabbing();

       

	}
    public void GuardianGrabbing(){

        GuardianAnim.SetBool("isGuardianGrabbing", true);
    }
    public void GuardianJumping(){

        GuardianAnim.SetBool("isGuardianJumping", true);
    }
    public void GuardianWalking(){

        GuardianAnim.SetBool("isGuardianWalking", true);
    }
    public void GuardianIdle(){

        GuardianAnim.SetBool("isGuardianIdle", true);
    }
}
