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
            Debug.LogError("Test E Keydown");
            PPanim.SetBool("isWarpingForward", true);
            Debug.LogError(PPanim.GetBool("isWarpingForward"));
        }
        if (Input.GetKeyUp(KeyCode.E)) {
            Debug.LogError("Test E Keyup");
            PPanim.SetBool("isWarpingForward", false);
            Debug.LogError(PPanim.GetBool("isWarpingForward"));
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            Debug.LogError("Test Q Keydown");
            PPanim.SetBool("isWarpingBackward", true);
            Debug.LogError(PPanim.GetBool("isWarpingBackward"));
        }
        if (Input.GetKeyUp(KeyCode.Q)) {
           Debug.LogError("Test Q Key up");
            PPanim.SetBool("isWarpingBackward", false);
            Debug.LogError(PPanim.GetBool("isWarpingBackward"));
        }
    }

    void debugging() {
        Debug.LogError("Warping");
    }
}
