using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingCrystals : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    private GameController _game;


    public void OnTriggerEnter(Collider Col)
    {
        if (Col.gameObject.tag == "Collectable" && _player.Team == Constants.Team.Miner)
        {
            if (_game != null) _game.IncrementMinerScore();
            Destroy(Col.gameObject);
        }
    }

    public void SetGame(GameController game) { _game = game; }
}
