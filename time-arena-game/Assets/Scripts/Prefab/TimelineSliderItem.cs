using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineSliderItem : MonoBehaviour
{
    [SerializeField] private Image _sliderIcon;

    public void SetUp(Sprite playerIcon) {
        _sliderIcon.sprite = playerIcon;

        // Reset its position and scale
        gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

}
