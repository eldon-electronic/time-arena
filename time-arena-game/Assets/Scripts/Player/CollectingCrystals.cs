using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollectingCrystals : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    private SceneController _sceneController;
    private GameController _game;

    public void Awake()
    {
        if (_player.Team == Constants.Team.Guardian) Destroy(this);
    }

    public void OnEnable()
    {
        GameController.gameActive += SetGame;
    }

    public void OnDisable()
    {
        GameController.gameActive -= SetGame;
    }

    public void Start()
    {
        _sceneController = FindObjectOfType<PreGameController>();
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Collectable")
        {
          PhotonView viewOfCrystal = col.gameObject.GetComponent<PhotonView>();
          PhotonView viewOfPlayer = gameObject.GetComponent<PhotonView>();
            if(viewOfCrystal.IsMine && _game != null) {
              viewOfCrystal.RPC("RPC_Collect", RpcTarget.All, viewOfPlayer.ViewID);
              //col.gameObject.GetComponent<CrystalBehaviour>().Collect();
            }
        }
    }

    private void SetGame(GameController game) { _sceneController = game; }
}
