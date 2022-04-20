using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudCooldowns : MonoBehaviour
{
    [SerializeField] private TimeConn _timeConn;
    [SerializeField] private Animator _backButtonAnimator;
    [SerializeField] private Animator _forwardButtonAnimator;
    private bool _isBackButtonSpinning;
    private bool _isForwardButtonSpinning;

    void Awake()
    {
        _isForwardButtonSpinning = false;
        _isBackButtonSpinning = false;
    }

    void LateUpdate()
    {
        (bool forward, bool back) canJump = _timeConn.GetCanJump();

        if (canJump.back && !_isBackButtonSpinning)
        {
            // Ready to jump after cooldown.
            _backButtonAnimator.SetBool("Rotate", true);
            _isBackButtonSpinning = true;
        }
        else if (!canJump.back && _isBackButtonSpinning)
        {
            // Cooldown activated.
            _backButtonAnimator.SetBool("Rotate", false);
            _isBackButtonSpinning = false;
        }

        if (canJump.forward && !_isForwardButtonSpinning)
        {
            _forwardButtonAnimator.SetBool("Rotate", true);
            _isForwardButtonSpinning = true;
        }
        else if (!canJump.forward && _isForwardButtonSpinning)
        {
            _forwardButtonAnimator.SetBool("Rotate", false);
            _isForwardButtonSpinning = false;
        }
    }
}
