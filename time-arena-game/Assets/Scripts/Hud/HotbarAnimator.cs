using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarAnimator : MonoBehaviour
{
    [SerializeField] private Animator _backButtonAnimator;
    [SerializeField] private Animator _forwardButtonAnimator;

    public void RotateBackButton() {
        _backButtonAnimator.SetBool("Rotate", true);
    }

    public void StopBackButton() {
        _backButtonAnimator.SetBool("Rotate", false);
    }

    public void RotateForwardButton() {
        _forwardButtonAnimator.SetBool("Rotate", true);
    }

    public void StopForwardButton() {
        _forwardButtonAnimator.SetBool("Rotate", false);
    }
}
