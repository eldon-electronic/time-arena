using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HudTutorial : MonoBehaviour
{
    public GameObject Tutorial;
    [SerializeField] private GameObject _popUpText;
    [SerializeField] private GameObject _ui;
    [SerializeField] private GameObject[] _tutorialObjects;
    // private GameObject _crystal;

    private Dictionary<string, GameObject> _tutorialArrows;

    void Awake()
    {
        //_crystal = GameObject.Find("TutorialTimeCrystal");
        _tutorialArrows = new Dictionary<string, GameObject>();
        foreach (GameObject tutorialObject in _tutorialObjects) {
        _tutorialArrows.Add(tutorialObject.name, tutorialObject);
        }
    }
   
   
    void OnEnable()
    {
        GameController.gameActive += OnGameActive;
    }

    void OnDisable()
    {
        GameController.gameActive -= OnGameActive;
    }

    private void OnGameActive(GameController game)
    {
        Destroy(Tutorial);
        Destroy(this);
    }
    
    public void SetMessage(string message)
    {
        _popUpText.GetComponent<TextMeshProUGUI>().text = message;
    }
    
    public void SetActive(bool value)
    {
        Tutorial.SetActive(value);
    }
     public void SetVisibilityUI(bool visibility)
    {
        _ui.SetActive(visibility);
    }
    public void SetVisibilityArrow(string uiElement,bool arrowVis)
    {
        if(_tutorialArrows.ContainsKey(uiElement)){
            _tutorialArrows[uiElement].SetActive(arrowVis);
        }
    }
 
 }

