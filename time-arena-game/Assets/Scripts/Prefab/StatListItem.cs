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

    public void SetUp(Constants.Team team, string playerNickname) {
        _nickname.text = playerNickname;
        switch (team) {
            case Constants.Team.Miner:
                _statsText.text = _minerTxt;
                break;
            case Constants.Team.Guardian:
                _statsText.text = _guardianTxt;
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
