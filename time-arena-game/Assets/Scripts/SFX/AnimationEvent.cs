using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    [SerializeField] private GameObject _soundObject;
    [SerializeField] private AudioClip _walkSound1;
    [SerializeField] private AudioClip _walkSound2;
    [SerializeField] private AudioClip _jumpSound;

    public void WalkSFXLeft() {
        AudioSource audio = _soundObject.GetComponent<AudioSource>();
        audio.PlayOneShot(_walkSound1);
    }

    public void WalkSFXRight() {
        AudioSource audio = _soundObject.GetComponent<AudioSource>();
        audio.PlayOneShot(_walkSound2);
    }

    public void JumpSFX() {
        AudioSource audio = _soundObject.GetComponent<AudioSource>();
        audio.PlayOneShot(_jumpSound);
    }

}
