using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class postTestScript : MonoBehaviour
{
    //public Animator PPanim;
    //public GameObject ppCont;
    PostProcessingControl controller;
    bool found = false;
    // Start is called before the first frame update
    void Start()
    {
        //thisScene = gameObject.scene;
        //controller = GameObject.FindGameObjectsWithTag("VolumeControl")[0].PostProcessingControl;
        GameObject temp = GameObject.Find("/PPControl");
        if(temp != null) {            
            controller = temp.GetComponent<PostProcessingControl>();
            found = true;
            Debug.LogError("Found compt");
        }
        else {
            Debug.LogError("not found compt");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(found)
        {
            if (Input.GetKeyDown(KeyCode.T)) {
                controller.StartAnim();
                Debug.LogError("just played anim");
            }
            if (Input.GetKeyUp(KeyCode.T)) {
            
            }
            if (Input.GetKeyDown(KeyCode.T)) {
            
            }
            if (Input.GetKeyUp(KeyCode.T)) {

            }
        }
    }

    void debugging() {
        Debug.LogError("Warping");
    }
}
