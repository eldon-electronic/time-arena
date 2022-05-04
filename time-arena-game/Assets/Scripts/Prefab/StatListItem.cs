using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _nickname;
    [SerializeField] private TMP_Text _statsText;

    private string _minerTxt = "Crystals collected: ";
    private string _guardianTxt = "Miners grabbed: ";
    private int _score;

    public void SetUp(string playerNickname, Constants.Team team, int score) {
        _nickname.text = playerNickname;
        _score = score;
        switch (team) {
            case Constants.Team.Miner:
                _statsText.text = _minerTxt + score.ToString();
                break;
            case Constants.Team.Guardian:
                _statsText.text = _guardianTxt + score.ToString();
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
