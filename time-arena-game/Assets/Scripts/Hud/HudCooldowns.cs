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
    [SerializeField] private Sprite _greenPressedSprite;
    [SerializeField] private Sprite _greenUnpressedSprite;
    [SerializeField] private Sprite _redPressedSprite;

    private float[] _cooldowns;
    private bool _canJumpForward;
    private bool _canJumpBack;

    void Start()
    {
        _cooldowns = new float[2];
        _canJumpForward = false;
        _canJumpBack = false;
    }

    void LateUpdate()
    {
        _forwardCooldownSlider.value = _cooldowns[0];
        _backCooldownSlider.value = _cooldowns[1];

        if (_canJumpForward) _forwardJumpIcon.sprite = _greenUnpressedSprite;
        else _forwardJumpIcon.sprite = _redPressedSprite;
        if (_canJumpBack) _backJumpIcon.sprite = _greenUnpressedSprite;
        else _backJumpIcon.sprite = _redPressedSprite;
    }

    public void SetCooldownValues(float[] items)
    {
        // Each item should be a float between 0.0f (empty) and 1.0f (full).
        _cooldowns = items;
    }

    public void SetCanJump(bool forward, bool back)
    {
        _canJumpForward = forward;
        _canJumpBack = back;
    }

    public void PressForwardJumpButton()
    {
        _forwardJumpIcon.sprite = _greenPressedSprite;
    }

    public void PressBackJumpButton()
    {
        _backJumpIcon.sprite = _greenPressedSprite;
    }
}
