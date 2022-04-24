using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudScore : MonoBehaviour
{
    [SerializeField] private Text _playerText;
    [SerializeField] private Text _teamText;

    void OnEnable()
    {
        SceneController.scoreChange += SetScore;
    }

    void OnDisable()
    {
        SceneController.scoreChange -= SetScore;
    }

    private void SetScore(int pScore, int tScore)
    {
        _playerText.text = pScore + "";
        _teamText.text = tScore + "";
    }
}
