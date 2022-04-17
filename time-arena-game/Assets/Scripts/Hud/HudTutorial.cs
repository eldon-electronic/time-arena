using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HudTutorial : MonoBehaviour
{
    [SerializeField] private PhotonView _view;
    [SerializeField] private GameObject _tutorial;
    [SerializeField] private GameObject _arrowImage;
    [SerializeField] private GameObject _popUpText;
    [SerializeField] private GameObject _optionsPopUpText;
    [SerializeField] private GameObject[] _tutorialObjects;

    private Dictionary<string, GameObject> _tutorialArrows;

    void Awake()
    {
        _tutorialArrows = new Dictionary<string, GameObject>();
        foreach (GameObject tutorialObject in _tutorialObjects) {
            _tutorialArrows.Add(tutorialObject.name, tutorialObject);
        }
    }

    public void SetVisibility(bool visibility) { _tutorial.SetActive(visibility); }

    public void SetArrowVisibility(string uiElement, bool visibility)
    {
        if (_tutorialArrows.ContainsKey(uiElement)) {
            _tutorialArrows[uiElement].SetActive(visibility);
        }
    }

    public void SetMessage(string message)
    {
        _popUpText.GetComponent<TextMeshProUGUI>().text = message;
    }

    public void SetOptionsText(string message)
    {
        _optionsPopUpText.GetComponent<TextMeshProUGUI>().text = message;
    }
}
