using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private Button _chooseTeamButton;
    [SerializeField] private GameObject _chooseTeamContainer;
    [SerializeField] private Launcher _launcher;
    public Image teamImage;

    private Player _player;
    private bool _displayingTeamSelector;

    public string username;

    public void SetUp(Player player, bool isMasterClient) { 
        _displayingTeamSelector = false;
        _player = player;
        _playerName.text = player.NickName;
        username = player.NickName;
        _chooseTeamButton.gameObject.SetActive(isMasterClient);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        if (_player ==  otherPlayer) {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom() {
        Destroy(gameObject);
    }

    public void DisplayTeamSelector() {
        if (!_displayingTeamSelector) {
            _chooseTeamContainer.gameObject.SetActive(true);
            _displayingTeamSelector = true;
        } else {
            _chooseTeamContainer.gameObject.SetActive(false);
            _displayingTeamSelector = false;
        }
    }

    public void SelectTeam() {
        teamImage.sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
        DisplayTeamSelector(); // disables the menu
        Launcher.Instance.UpdateIcons(this.username, this.teamImage.sprite.name);
    }

    public void UpdateMasterClientOptions(bool isMasterClient) {
        _chooseTeamButton.gameObject.SetActive(isMasterClient);
    }
}
