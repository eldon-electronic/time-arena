using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// This script sets a name tag above other players using their nickname
public class PlayerNameTag : MonoBehaviourPun
{
    [SerializeField] private PhotonView _view;
    [SerializeField] private GameObject _nameTag;
    [SerializeField] private TextMeshProUGUI _nameText;

    void Awake()
    {
        if (_view.IsMine)
        {
            Destroy(_nameTag);
            Destroy(this);
        }
        _nameText.text = photonView.Owner.NickName;
    }

    void Update()
    {
        _nameTag.SetActive(gameObject.layer == Constants.LayerPlayer);
    }
}
