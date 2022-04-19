using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelector : MonoBehaviour
{

    [SerializeField] private GameObject _iconContainer;
    private bool _isDisplaying;

    void Awake()
    {
        _isDisplaying = false;
    }

    public void DisplayTeamIcons() {
        if (!_isDisplaying) {
            _iconContainer.SetActive(true); 
            _isDisplaying = true;
        } else {
            _iconContainer.SetActive(false);
            _isDisplaying = false;
        }
    }
}
