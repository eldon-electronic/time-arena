using UnityEngine;
using UnityEngine.UI;

public class TimelineSliderItem : MonoBehaviour
{
    [SerializeField] private Image _sliderIcon;
    [SerializeField] private Image _iconStroke;
    [SerializeField] private GameObject _handle;
    [SerializeField] private GameObject _handleSliderArea;

    public void SetUp(Sprite playerIcon, bool isMe) {
        _sliderIcon.sprite = playerIcon;

        // If it's yours, activate stroke and resize the handle
        if (isMe) {
            _iconStroke.gameObject.SetActive(true);
            _handle.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 0);
            _handleSliderArea.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            _handleSliderArea.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        }

        // Reset its position and scale
        gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
}
