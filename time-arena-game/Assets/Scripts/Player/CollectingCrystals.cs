using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingCrystals : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    private SceneController _sceneController;

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
            _sceneController?.IncrementMinerScore();
            Destroy(col.gameObject);
        }
    }

    private void SetGame(GameController game) { _sceneController = game; }
}
