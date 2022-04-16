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

    void Start()
    {
        _nameText.text = photonView.Owner.NickName;
        if (_view.IsMine) Destroy(_nameTag);
    }

    void Update()
    {
        if (_view.IsMine) return;

        if (gameObject.layer == Constants.LayerOutsideReality)
		{
			_nameTag.SetActive(false);
		}
		else _nameTag.SetActive(true);
    }
}
