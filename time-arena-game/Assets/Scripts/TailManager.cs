using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    private TimeLord _timeLord;
    private Dictionary<int, TailController> _tails;

    void Start()
    {
        _tails = new Dictionary<int, TailController>();
    }

    // Creates tail objects for any tails that come into existance on this frame. 
    private void CreateNewTails()
	{
		List<PlayerState> states = _timeLord.GetCreatedTails();
		if (states != null)
		{
			foreach (var state in states)
			{
				GameObject tail = (GameObject) Resources.Load("rePlayer");
				TailController tailController = tail.GetComponent<TailController>();
				tailController.Initialise(state, _timeLord);
				_tails.Add(state.TailID, tailController);
			}
		}
	}

    void Update()
    {
        if (_timeLord != null) CreateNewTails();
    }


    // ------------ PUBLIC FUNCTIONS ------------

    public void SetTimeLord(TimeLord timeLord) { _timeLord = timeLord; }

    public void DestroyTails()
    {
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
			GameObject tailObject = (GameObject) Resources.Load("rePlayer");
			TailController tailController = tailObject.GetComponent<TailController>();
			tailController.Initialise(tail.Value, _timeLord);
			_tails.Add(tail.Key, tailController);
		}
    }
}
