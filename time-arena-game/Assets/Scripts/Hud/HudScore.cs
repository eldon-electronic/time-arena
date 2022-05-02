using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudScore : MonoBehaviour
{
    [SerializeField] private Text _playerText;
    [SerializeField] private Text _teamText;

    void Awake()
    {
        _playerText.text = "0";
        _teamText.text = "0";
    }

    void OnEnable()
    {
        SceneController.scoreChange += SetTeamScore;
    }

    void OnDisable()
    {
        SceneController.scoreChange -= SetTeamScore;
    }

    private void SetTeamScore(int score) { _teamText.text = score + ""; }

    public void SetYourScore(int score) { _playerText.text = score + ""; }
}
