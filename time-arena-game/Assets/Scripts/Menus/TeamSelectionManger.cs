using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectionManger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Button _rightButton;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _selectButton;
    [SerializeField] private TMP_Text _teamText;
    [SerializeField] private GameObject _minerUI;
    [SerializeField] private GameObject _guardianUI;
    private string _team;
    public static Constants.Team ChooseTeam;
    

    void Start()
    {
        
        _minerUI.gameObject.SetActive(true);
        _guardianUI.gameObject.SetActive(false);
        _rightButton.gameObject.SetActive(true);
        _leftButton.gameObject.SetActive(false);
        _team = "Miner";
        _teamText.text = _team;
        ChooseTeam = Constants.Team.Miner;
    }
    

    // Update is called once per frame
   
    public void LeftClick(){

        _rightButton.gameObject.SetActive(true);
        _leftButton.gameObject.SetActive(false);
        _minerUI.gameObject.SetActive(true);
        _guardianUI.gameObject.SetActive(false);
        _team = "Miner";
        _teamText.text = _team;
        ChooseTeam = Constants.Team.Miner;
        //Debug.Log(ChooseTeam);

    }
    public void RightClick(){

        _rightButton.gameObject.SetActive(false);
        _leftButton.gameObject.SetActive(true);
        _minerUI.gameObject.SetActive(false);
        _guardianUI.gameObject.SetActive(true);
        _team = "Guardian";
        _teamText.text = _team;
        ChooseTeam = Constants.Team.Guardian;
        //Debug.Log(ChooseTeam);

    }
    
    
  
   
}

