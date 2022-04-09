using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingCrystals : MonoBehaviour
{
    public PlayerHud hud;
    [SerializeField] private PlayerController player;


    public void OnTriggerEnter(Collider Col)
    {
        // TODO: Get team from PlayerController.
        if (Col.gameObject.tag == "Collectable" && player.Team == Constants.Team.Miner)
        {
            Debug.Log("Crystal collected!");
            hud.setScore(hud.getScore()+1);
            //Col.gameObject.SetActive(false);
            Destroy(Col.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
