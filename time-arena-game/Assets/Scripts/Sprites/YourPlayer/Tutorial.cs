using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    class State
    {
        public string Message;
        public float Delay;
        public State(string message,float delay)
        {
            Message = message;
            Delay = delay;
        }
    }
    
    [SerializeField] private PlayerController _player;
    [SerializeField] private HudTutorial _tutorialHud;
    [SerializeField] private PhotonView _view;
    [SerializeField] private GameObject _masterClientOptions;

    private bool _hasMovedOn = true;
    private List<State> _guardianStates;
    private List<State> _minerStates;
    private List<State> _states;
    private int _currentState;
    private float _delayTime;
   
  

    // ------------ UNITY FUNCTIONS ------------

    void Awake()
    {
        _masterClientOptions.SetActive(false);
        if (!_view.IsMine) Destroy(this);
        CreateStatesGuardian();
        CreateStatesMiner();
    }

    void OnEnable() { GameController.gameActive += OnGameActive; }

    void OnDisable() { GameController.gameActive -= OnGameActive; }

    void Start()
    {
        if (_player.Team == Constants.Team.Guardian) _states = _guardianStates;
        else _states = _minerStates;
        StartTutorial();
    }

    void Update()
    {
        if (_currentState < _states.Count)
        {
            if (_hasMovedOn)
            {
                StartCoroutine(DelayPopup());
                _hasMovedOn = false;     
            }
            
        }
    }


    // ------------ ON EVENT METHODS ------------

    private void OnGameActive(GameController game) { Destroy(this); }


    // ------------ PRIVATE METHODS ------------

    private void CreateStatesGuardian()
    {
        _guardianStates = new List<State>();
        _guardianStates.Add(new State("Welcome to tutorial Guardian,\n\n Your aim is to catch the Miners and steal their crystals!",2));
        _guardianStates.Add(new State("This is the growing crystal obstacle, it only appears at certain times in the game.",8));
        _guardianStates.Add(new State("You should travel backwards in time to pass it.",12));
        _guardianStates.Add(new State("This is the braking crystal obstacle, it only breaks at certain times in the game.",2 ));
        _guardianStates.Add(new State("You should travel forwards in time to pass it.",4));
        _guardianStates.Add(new State("And this is you!,Let's start playing!",5));
    }

    private void CreateStatesMiner()
    {
        _minerStates = new List<State>();
        _minerStates.Add(new State("Welcome to tutorial Miner,\n\nYour aim is to collect crystals and run away from Guardians!",12));
        _minerStates.Add(new State("This the collectable crystal, you should run through it to collect.",2));
        _minerStates.Add(new State("WARNING:It only appears at certain times in the game\n\n You should time travel to find them.",4));
        _minerStates.Add(new State("This is the growing crystal obstacle, it only appears at specific times in the game.",10));
        _minerStates.Add(new State("You should travel backwards in time to pass it",3));
        _minerStates.Add(new State("This is the breaking crystal obstacle, it only breaks at specific times in the game.",15));
        _minerStates.Add(new State("You should travel forwards in time to pass it",2));
        _minerStates.Add(new State("And this is you!,Let's start playing!",20));
    }

    private void StartTutorial()
    {
        _currentState = 0;
        _tutorialHud.SetMessage(_states[_currentState].Message);
        SetDelayTime(_states[_currentState].Delay);
    }

    private void MoveToState(int state)
    {
        if (state >= _states.Count) return;
        // Set the new state.
        _currentState = state;
        _tutorialHud.SetMessage(_states[_currentState].Message);
        SetDelayTime(_states[_currentState].Delay);
      
        // If master client start the game.
        /*if (state == _states.Count - 1){
            if (PhotonNetwork.IsMasterClient) _masterClientOptions.SetActive(true);
        } */
    }

    IEnumerator DelayPopup() {
        yield return new WaitForSeconds(_delayTime);
        MoveToState(_currentState + 1);
        _hasMovedOn = true;

    }
    public void SetDelayTime(float tutorialDelay)
    {
       _delayTime = tutorialDelay;
    }
 }

