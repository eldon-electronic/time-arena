using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFXController : MonoBehaviour
{

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _buttonSFX;

    public void PlaySFX() {
        _audioSource.PlayOneShot(_buttonSFX);
    }
    
}
