using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollectingCrystals : MonoBehaviour
{
  [SerializeField] private PlayerController _player;
  [SerializeField] private CrystalManager cm;
    private GameController _game;



    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Collectable" && _player.Team == Constants.Team.Miner)
        {
            if (_game != null) _player.incrScore();
            cm.collected(col.gameObject.GetComponent<CrystalBehaviour>().ID);
        }
    }

    public void SetGame(GameController game) {
      _game = game;
      cm = game.gameObject.GetComponent<CrystalManager>();
    }
}
