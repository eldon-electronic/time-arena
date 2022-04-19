using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudTimeline : MonoBehaviour
{
    [SerializeField] private GameObject _timeline;
    [SerializeField] private Image _timelineFill;
    [SerializeField] private Slider _yourIcon;
    [SerializeField] private Slider _playerIcon1;
    [SerializeField] private Slider _playerIcon2;
    [SerializeField] private Slider _playerIcon3;
    [SerializeField] private Slider _playerIcon4;
    private TimeLord _timeLord;
    private Slider[] _playerIcons;
    private List<float> _playerPositions;

    void Awake()
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
        _timelineFill.fillAmount = _timeLord.GetTimeProportion();
        _yourIcon.value = _timeLord.GetYourPosition();
        List<float> players = _timeLord.GetPlayerPositions();
        for (int i=0; i < players.Count; i++)
        {
            _playerIcons[i].value = players[i];
        }
    }

    public void SetTimeLord(TimeLord timeLord) { _timeLord = timeLord; }
}
