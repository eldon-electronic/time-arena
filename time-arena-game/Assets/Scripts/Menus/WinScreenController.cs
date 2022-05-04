using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class WinScreenController : MonoBehaviour
{

    public static WinScreenController Instance;

    [SerializeField] private GameObject _statsContainer;
    [SerializeField] private GameObject _statListItem;
    private Player[] players;

    void Awake() {
        Instance = this;
    }

    public void Activate() {
        
    }

}
