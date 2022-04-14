using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    private TimeLord _timeLord;
    private Dictionary<int, TailController> _tails;
    private bool _activated;
    [SerializeField] private GameObject _tailPrefab;
    private bool _particlesEnabled;

    void Awake()
    {
        _tails = new Dictionary<int, TailController>();
        _activated = false;
        _particlesEnabled = true;
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


    // ------------ PUBLIC FUNCTIONS FOR PLAYER CONTROLLER ------------

    public void SetTimeLord(TimeLord timeLord) { _timeLord = timeLord; }

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
}
