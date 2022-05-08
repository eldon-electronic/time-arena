using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;


// This script is used to control the rotation of an object so that it always faces the main camera
public class Billboard : MonoBehaviour
{
    private Transform _mainCameraTransform;
    private int _waitCount;

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
                _waitCount++;
                if (_waitCount % 100 == 0) Debug.Log($"Waiting for main camera exceeded {_waitCount} frames.");
            }
        }
    }

    private void OnClientEntered(PlayerController _)
    { 
        _mainCameraTransform = Camera.main.transform;
    }
}
