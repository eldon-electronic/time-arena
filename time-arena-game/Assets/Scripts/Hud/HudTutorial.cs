using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HudTutorial : MonoBehaviour
{
    [SerializeField] private GameObject _tutorial;
    [SerializeField] private GameObject _arrowImage;
    [SerializeField] private GameObject _popUpText;
    [SerializeField] private GameObject _optionsPopUpText;
    private Dictionary<string, Vector2> _uiPositions;
    private Dictionary<string, Vector3> _uiRotations;

    void Start()
    {
        _uiPositions = new Dictionary<string, Vector2>();
        _uiPositions.Add("forwardJump",new Vector2(381f,-76f));
        _uiPositions.Add("backJump",new Vector2(-397f,-82f));
        _uiPositions.Add("timebar",new Vector2(-342f,-122f));
        _uiPositions.Add("timer",new Vector2(-290f,105f));
        _uiPositions.Add("team",new Vector2(284f,104f));


        _uiRotations = new Dictionary<string, Vector3>();
        _uiRotations.Add("forwardJump",new Vector3(-10.1f,-720.2f,18.5f));
        _uiRotations.Add("backJump",new Vector3(-10.1f,-535.5f,4.7f));
        _uiRotations.Add("timebar",new Vector3(-10.1f,-535.5f,4.7f));
        _uiRotations.Add("timer",new Vector3(-10.1f,-535.5f,-150f));
        _uiRotations.Add("team",new Vector3(-10.1f,-355.5f,-150f));
    }

    void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name != "PreGameScene")
        {
            _tutorial.SetActive(false);
        }
    }

    public void SetVisibility(bool visibility) { _tutorial.SetActive(visibility); }

    public void SetArrowPosition(string uiElement)
    {
        _arrowImage.GetComponent<RectTransform>().anchoredPosition = _uiPositions[uiElement];
        _arrowImage.GetComponent<RectTransform>().eulerAngles = _uiRotations[uiElement];
    }

    public void SetArrowVisibility(bool visibility)
    {
        _arrowImage.SetActive(visibility);
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
