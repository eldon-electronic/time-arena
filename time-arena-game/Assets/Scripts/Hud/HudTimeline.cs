using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudTimeline : MonoBehaviour
{
    [SerializeField] private GameObject _timeline;
    [SerializeField] private Slider _elapsedTimeSlider;
    [SerializeField] private Slider _yourIcon;
    [SerializeField] private Slider _playerIcon1;
    [SerializeField] private Slider _playerIcon2;
    [SerializeField] private Slider _playerIcon3;
    [SerializeField] private Slider _playerIcon4;
    private Slider[] _playerIcons;
    private float _timeBarPosition;
    private float _yourPosition;
    private List<float> _playerPositions;

    void Start()
    {
        _playerPositions = new List<float>();
        _playerIcons = new Slider[] {_playerIcon1, _playerIcon2, _playerIcon3, _playerIcon4};
    }

    void LateUpdate()
    {
        // Set visibility of timeline, player icons and jump cooldowns.
        // TimelineCanvasGroup.alpha = (SceneManager.GetActiveScene().name != "PreGameScene") ? 1.0f: 0.0f;
        // ElapsedTimeSlider.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
        // _playerIcons[0].gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
        // for (int i=0; i < _playerPositions.Count; i++)
        // {
        //     _playerIcons[i].gameObject.SetActive(true);
        // }

        // Set player icon positions.
        _elapsedTimeSlider.value = _timeBarPosition;
        _yourIcon.value = _yourPosition;
        for (int i=0; i < _playerPositions.Count; i++)
        {
            _playerIcons[i].value = _playerPositions[i];
        }
    }

    public void SetActive(bool value) { _timeline.SetActive(value); }

    public void SetTimeBarPosition(float value) { _timeBarPosition = value; }

    public void SetPlayerPositions(float clientPosition, List<float> playerPositions)
    {
        _yourPosition = clientPosition;
        _playerPositions = playerPositions;
    }
}
