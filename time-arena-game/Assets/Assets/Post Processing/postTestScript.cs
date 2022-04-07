using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class postTestScript : MonoBehaviour
{
    public Animator PPanim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            PPanim.SetBool("isWarpingForward", true);
        }
        if (Input.GetKeyUp(KeyCode.E)) {
            PPanim.SetBool("isWarpingForward", false);
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            PPanim.SetBool("isWarpingBackward", true);
        }
        if (Input.GetKeyUp(KeyCode.Q)) {
            PPanim.SetBool("isWarpingBackward", false);
        }
    }
}
