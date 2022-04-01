using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingCrystals : MonoBehaviour
{
    public PlayerHud hud;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnTriggerEnter(Collider Col)
    {
        if(Col.gameObject.tag == "Collectable" && hud.TeamDispl.text == "HIDER")
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
