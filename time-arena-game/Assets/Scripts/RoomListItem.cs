using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text _roomText;
    public RoomInfo Info;

    public void SetUp(RoomInfo roomInfo) { 
        Info = roomInfo;
        _roomText.text = roomInfo.Name;
    }

    public void OnClick() {
        Launcher.Instance.JoinRoom(Info);
    }
}
