using UnityEngine;


// This script is used to control the rotation of an object so that it always faces the main camera
public class Billboard : MonoBehaviour
{

    private Transform mainCameraTransform;

    void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (transform != null) {
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation * Vector3.up);
        }
    }
}
