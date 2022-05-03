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
    [SerializeField] private Image _teamImage;

    private Player _player;
    private bool _displayingTeamSelector;
    private string _userID;

    public string username;

    public void SetUp(Player player, bool isMasterClient, Sprite[] allIcons, string iconName = "no_team_icon") { 
        _displayingTeamSelector = false;
        _player = player;
        _playerName.text = player.NickName;
        username = player.NickName;
        _userID = player.UserId;

        foreach (Sprite icon in allIcons) {
            if (iconName == icon.name) _teamImage.sprite = icon;
        }

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
        _teamImage.sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
        DisplayTeamSelector();
        Launcher.Instance.UpdateIcon(this._userID, this._teamImage.sprite.name);
    }

    public void UpdateMasterClientOptions(bool isMasterClient) {
        _chooseTeamButton.gameObject.SetActive(isMasterClient);
    }

    public string GetUserID() {
        return _userID;
    }

    public Image GetTeamImage() {
        return _teamImage;
    }

    public void SetTeamImage(Sprite newImage) {
        _teamImage.sprite = newImage;
    }
}
