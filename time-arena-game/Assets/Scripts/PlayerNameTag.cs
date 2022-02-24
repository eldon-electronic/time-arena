using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// This script sets a name tag above other players using their nickname
public class PlayerNameTag : MonoBehaviourPun
{

    [SerializeField] private TextMeshProUGUI nameText;

    void Start()
    {
        if (photonView.IsMine) { return; }

        nameText.text = photonView.Owner.NickName;
    }

}
