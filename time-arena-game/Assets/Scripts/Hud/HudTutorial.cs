using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HudTutorial : MonoBehaviour
{
    [SerializeField] private GameObject _tutorial;
    [SerializeField] private GameObject _popUpText;
    [SerializeField] private GameObject _optionsPopUpText;
    [SerializeField] private GameObject[] _tutorialObjects;
    [SerializeField] private Text _scorePlayer;
    [SerializeField] private Text _scoreTeam;
    private GameObject _crystal;
    private SceneController _sceneController;

    private Dictionary<string, GameObject> _tutorialArrows;

    void Awake()
    {
        _crystal = GameObject.Find("TutorialCrystal");
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
        Destroy(_tutorial);
        Destroy(this);
        ResetScore();

    }

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


    public void SetCrystalVisibility(bool crystalVis){
        
        if (SceneManager.GetActiveScene().name == "PreGameScene"){

            _crystal.SetActive(crystalVis);
        }

    }
    public void UpdateTutorialScore(bool tutScore){

        if (SceneManager.GetActiveScene().name != "GameScene"){
            if(tutScore == true){
                _scorePlayer.text = "1";
                _scoreTeam.text = "1";
            }
        }
    }
    public void ResetScore(){
        _scorePlayer.text = "0";
        _scoreTeam.text = "0";
    }

 }

