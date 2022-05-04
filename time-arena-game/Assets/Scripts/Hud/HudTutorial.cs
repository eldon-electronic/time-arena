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
    

 }

