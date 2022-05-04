using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudKeyPanel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject _keyPanel;
    [SerializeField] private GameObject _keyPanelText;
    [SerializeField] private GameObject _hideText;
    [SerializeField] private GameObject _showText;
    [SerializeField] private PlayerController _player;
    [SerializeField] private PhotonView _view;
   

    void Awake()
    {
        if (!_view.IsMine) Destroy(this);
        _keyPanel.SetActive(true);  
    }

    void Start()
    {
        if(_player.Team == Constants.Team.Guardian){
            SetKeyPanelText("<sprite=9> TO MOVE\n\n\n<sprite=15> TO JUMP\n\n\n<sprite=2> + <sprite=12> TO SPRINT\n\n\nHOLD <sprite=7> TO TRAVEL BACK\n\n\nHOLD <sprite=14> TO TRAVEL FORWARD\n\n\n<sprite=23> CLICK TO GRAB");
        }
        else if(_player.Team == Constants.Team.Miner){
            SetKeyPanelText("<sprite=9> TO MOVE\n\n\n<sprite=15> TO JUMP\n\n\n<sprite=2> + <sprite=12> TO SPRINT\n\n\nHOLD <sprite=7> TO TRAVEL BACK\n\n\nHOLD <sprite=14> TO TRAVEL FORWARD");
        }
        _hideText.SetActive(true);
        _showText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H)){
            _keyPanel.SetActive(false);
            _hideText.SetActive(false);
            _showText.SetActive(true);
        }
        else if(Input.GetKeyDown(KeyCode.F)){
            _keyPanel.SetActive(true);
            _hideText.SetActive(true);
            _showText.SetActive(false);
        }
          
    }
    public void SetKeyPanelText(string keyText)
    {
        _keyPanelText.GetComponent<TextMeshProUGUI>().text = keyText;
    }     
}
