using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class CollectingCrystals : MonoBehaviour
{
    public PlayerHud hud;
    public PhotonView view;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnTriggerEnter(Collider Col)
    {
        // TODO: Get team from PlayerController.
        if(Col.gameObject.tag == "Collectable" && hud.TeamDispl.text == "MINER")
        {
            view.RPC("RPC_incrScore", RpcTarget.All);
            PhotonNetwork.Destroy(Col.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
