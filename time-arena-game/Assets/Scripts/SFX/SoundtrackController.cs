using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundtrackController : MonoBehaviour
{

    [SerializeField] private AudioSource _soundtrack;

    void OnEnable() 
    {
        GameController.gameStarted += OnGameStarted;
    }

    void OnDisable()
    {
        GameController.gameStarted -= OnGameStarted;
    }

    private void OnGameStarted() {
        _soundtrack.Play();
    }
}
