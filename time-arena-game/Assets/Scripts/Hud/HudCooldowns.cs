using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudCooldowns : MonoBehaviour
{
    [SerializeField] private Slider _forwardCooldownSlider;
    [SerializeField] private Slider _backCooldownSlider;
    [SerializeField] private Image _forwardJumpIcon;
    [SerializeField] private Image _backJumpIcon;
    [SerializeField] private Sprite _greenUnpressedSprite;
    [SerializeField] private Sprite _redPressedSprite;
    [SerializeField] private PlayerController _player;
    private float _forwardBarHeight;
    private float _backBarHeight;
    private bool _canJumpForward;
    private bool _canJumpBack;

    void Awake()
    {
        _forwardBarHeight = 0.0f;
        _backBarHeight = 0.0f;
        _canJumpForward = false;
        _canJumpBack = false;
    }

    void Update()
    {
        (float forward, float back) cooldowns = _player.GetCooldowns();
        _forwardBarHeight = 1.0f - (cooldowns.forward / 15.0f);
        _backBarHeight = 1.0f - (cooldowns.back / 15.0f);

        (bool forward, bool back) canJump = _player.GetCanJump();
        _canJumpForward = canJump.forward;
        _canJumpBack = canJump.back;
    }

    void LateUpdate()
    {
        _forwardCooldownSlider.value = _forwardBarHeight;
        _backCooldownSlider.value = _backBarHeight;

        if (_canJumpForward) _forwardJumpIcon.sprite = _greenUnpressedSprite;
        else _forwardJumpIcon.sprite = _redPressedSprite;
        if (_canJumpBack) _backJumpIcon.sprite = _greenUnpressedSprite;
        else _backJumpIcon.sprite = _redPressedSprite;
    }
}
