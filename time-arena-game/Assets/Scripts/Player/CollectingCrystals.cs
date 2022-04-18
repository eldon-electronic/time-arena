using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingCrystals : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
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

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Collectable")
        {
            _game?.IncrementMinerScore();
            Destroy(col.gameObject);
        }
    }

    private void SetGame(GameController game) { _game = game; }
}
