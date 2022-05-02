using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour, Debuggable
{
    [SerializeField] private GameObject _tailPrefab;
    [SerializeField] private PhotonView _view;
    [SerializeField] private HudDebugPanel _debugPanel;
    private TimeLord _timeLord;
    private Dictionary<int, TailController> _tails;
    private bool _activated;
    private bool _particlesEnabled;


    // ------------ UNITY METHODS ------------

    void Awake()
    {
        if (!_view.IsMine) Destroy(this);
        _tails = new Dictionary<int, TailController>();
        _activated = false;
        _particlesEnabled = true;
    }

    void OnEnable()
    {
        GameController.gameActive += OnGameActive;
        GameController.gameStarted += OnGameStarted;
        GameController.gameEnded += OnGameEnded;
    }

    void OnDisable()
    {
        GameController.gameActive -= OnGameActive;
        GameController.gameStarted -= OnGameStarted;
        GameController.gameEnded -= OnGameEnded;
    }

    void Start()
    {
        Debug.Log("TailManager Start");
        _timeLord = FindObjectOfType<SceneController>().GetTimeLord();
        _debugPanel.Register(this);
    }

    void Update()
    {
        // Create a new tail for any state on this frame that doesn't currently have one.
        if (_activated && _timeLord != null)
        {
            Dictionary<int, PlayerState> tails = _timeLord.GetTailStates();
            foreach (var tail in tails)
            {
                if (!_tails.ContainsKey(tail.Key))
                {
                    GameObject tailObject = Object.Instantiate(_tailPrefab);
                    TailController tailController = tailObject.GetComponent<TailController>();
                    tailController.Initialise(tail.Value, _timeLord, this);
                    _tails.Add(tail.Key, tailController);
                }
            }
        }
    }


    // ------------ ON EVENT FUNCTIONS ------------

    private void OnGameActive(GameController game)
    {
        _activated = false;
        _timeLord = game.GetTimeLord();
    }

    private void OnGameStarted() { _activated = true; }

    private void OnGameEnded(Constants.Team team) { _activated = false; }


    // ------------ PUBLIC FUNCTIONS FOR TIME CONN ------------

    public void SetActive(bool value) { _activated = value; }

    public void EnableParticles(bool value) { _particlesEnabled = value; }


    // ------------ PUBLIC FUNCTIONS FOR TAIL CONTROLLER ------------

    public void ReceiveResignation(int tailID)
    {
        // TODO: Remove this check, it shouldn't be necessary.
        if (_tails.ContainsKey(tailID))
        {
            _tails[tailID].Kill();
            _tails.Remove(tailID);
        }
    }

    public bool GetParticlesEnabled() { return _particlesEnabled; }


    // ------------ OTHER PUBLIC FUNCTIONS ------------

    public Hashtable GetDebugValues()
    {
        Hashtable debugItems = new Hashtable();
        foreach (var tail in _tails)
        {
            debugItems.Add($"tail {tail.Key}", true);
        }
		return debugItems;
    }
}
