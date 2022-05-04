using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;


// This script is used to control the rotation of an object so that it always faces the main camera
public class Billboard : MonoBehaviour
{
    private Transform _mainCameraTransform;

    void OnEnable() { PlayerController.clientEntered += OnClientEntered; }

    void OnDisable() { PlayerController.clientEntered -= OnClientEntered; }

    void LateUpdate()
    {
        if (transform != null)
        {
            if (_mainCameraTransform != null)
            {
                transform.LookAt(transform.position + _mainCameraTransform.rotation * Vector3.forward, _mainCameraTransform.rotation * Vector3.up);
            }
            else try
            {
                _mainCameraTransform = Camera.main.transform;
            }
            catch (NullReferenceException e)
            {
                Debug.Log("Waiting for main camera...");
                return;
            }
        }
    }

    private void OnClientEntered(PlayerController _)
    { 
        _mainCameraTransform = Camera.main.transform;
    }
}
