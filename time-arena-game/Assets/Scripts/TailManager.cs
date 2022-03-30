using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    private TimeLord _timeLord;
    private Dictionary<int, TailController> _tails;
    private bool _activated;
    [SerializeField] private GameObject _tailPrefab;

    void Start()
    {
        _tails = new Dictionary<int, TailController>();
        _activated = false;
    }

    // Creates tail objects for any tails that come into existance on this frame. 
    private void CreateNewTails()
	{
		List<PlayerState> states = _timeLord.GetCreatedTails();
		foreach (var state in states)
		{
			GameObject tail = (GameObject) Resources.Load("rePlayer");
			TailController tailController = tail.GetComponent<TailController>();
			tailController.Initialise(state, _timeLord, this);
			_tails.Add(state.TailID, tailController);
		}
	}

    void Update()
    {
        if (_activated && _timeLord != null)
        {
            CreateNewTails();
        }
    }


    // ------------ PUBLIC FUNCTIONS FOR PLAYER CONTROLLER ------------

    public void SetTimeLord(TimeLord timeLord) { _timeLord = timeLord; }

    public void DestroyTails()
    {
        if (!_activated) _activated = true;

        foreach (var tail in _tails)
		{
			tail.Value.Kill();
		}
		_tails = new Dictionary<int, TailController>();
    }

    public void BirthTails()
    {
        Dictionary<int, PlayerState> tails = _timeLord.GetAllTails();

		foreach (var tail in tails)
		{
            GameObject tailObject = Object.Instantiate(_tailPrefab);
			TailController tailController = tailObject.GetComponent<TailController>();
			tailController.Initialise(tail.Value, _timeLord, this);
			_tails.Add(tail.Key, tailController);
		}
    }

    public void Deactivate() { _activated = false; }


    // ------------ PUBLIC FUNCTIONS FOR TAIL CONTROLLER ------------

    public void ReceiveResignation(int tailID)
    {
        _tails[tailID].Kill();
        _tails.Remove(tailID);
    }
}
