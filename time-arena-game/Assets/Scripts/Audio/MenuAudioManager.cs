using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuAudioManager : MonoBehaviour
{

    [SerializeField] private AudioSource _menuStartClip;
    [SerializeField] private AudioSource _menuLoopClip;

    void Start()
    {
        _menuStartClip.Play();
        _menuLoopClip.PlayDelayed(_menuStartClip.clip.length);
    }
}
