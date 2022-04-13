using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    class State
    {
        public string Message;
        public string ElementToPointTo;
        public KeyCode InputTrigger;
        public bool VisibilityOfArrow;
        public bool NeedKey;
        public bool CrystalVisibility;

        public State(string message, string elementToPointTo, KeyCode inputTrigger ,bool visibilityOfArrow,bool needKeyPress,bool visibilityOfCrystal)
        {
            Message = message;
            ElementToPointTo = elementToPointTo;
            InputTrigger = inputTrigger;
            VisibilityOfArrow = visibilityOfArrow;
            NeedKey = needKeyPress;
            CrystalVisibility = visibilityOfCrystal;
        }
    }
    
    private bool _hasMovedOn = true;
    [SerializeField] private HudTutorial _tutorialHud;
    [SerializeField] private PhotonView _view;
    private List<State> _guardianStates;
    private List<State> _minerStates;
    private List<State> _states;
    private int _currentState;
  

    void Awake()
    {
        CreateStatesGuardian();
        CreateStatesMiner();
        _states = _minerStates;
    }


    void Update()
    {
        if (!_view.IsMine) return;
        if ((_currentState == (_states.Count-1))) StartTutorialOver();
        else if (_currentState < _states.Count-1) SkipTutorial();

        NeedKeyPress(_states[_currentState].NeedKey);
    }

    private void CreateStatesGuardian()
    {
        _guardianStates = new List<State>();
        _guardianStates.Add(new State("Welcome to tutorial Guardian!\n\nPlease use <sprite=9> keys to move around.","backJump", KeyCode.S,false,true));
        _guardianStates.Add(new State("Welcome to tutorial Miner!\n\nPlease use <sprite=26> keys to move around.","backJump", KeyCode.S,false,false));
        _guardianStates.Add(new State("Press <sprite=12> + <sprite=2> to sprint. ","backJump", KeyCode.W,false,true));
        _guardianStates.Add(new State("Press <sprite=29> + <sprite=21> to sprint. ","backJump", KeyCode.W,false,false));
        _guardianStates.Add(new State("Use <sprite=15> to jump.","backJump", KeyCode.Space,false,true));
        _guardianStates.Add(new State("Use <sprite=31> to jump.","backJump", KeyCode.Space,false,false));
        _guardianStates.Add(new State("Click right <sprite=13> to grab miners.","backJump", KeyCode.Mouse0,false,true));
        _guardianStates.Add(new State("Click right <sprite=23> to grab miners.","backJump", KeyCode.Space,false,false));
        _guardianStates.Add(new State("Now,let's have a look at game features!!","backJump", KeyCode.A,false,false));
        _guardianStates.Add(new State("This is the timer which shows the game time.\nYou have 5 minutes!!","timer", KeyCode.Return,true,false));
        _guardianStates.Add(new State("This shows the team you are in!!","team", KeyCode.Return,true,false));
        _guardianStates.Add(new State("This is the timebar which helps you to see where you are at in time.","timebar", KeyCode.Return,true,false));
        _guardianStates.Add(new State("IT'S TIME TO TIME TRAVEL!!","timebar", KeyCode.Return,false,false));
        _guardianStates.Add(new State("This icon shows the ability of time travelling backwards.\nOnce it turns to green you can travel back in time.", "backJump",KeyCode.Return,true,false));
        _guardianStates.Add(new State("Now, you are ready to go to the past!!\n\nPlease press <sprite=7>.", "backJump",KeyCode.Q,true,true));
        _guardianStates.Add(new State("Now, you are ready to go to the past!!\n\nPlease press <sprite=24>.", "backJump",KeyCode.Q,true,false));
        _guardianStates.Add(new State("Well done! You just traveled back in time!", "timebar",KeyCode.Return,false,false));
        _guardianStates.Add(new State("Let's travel forwards now!!\nThis icon shows the ability of time travelling forwards.\nOnce it turns to green you can travel forward in time.", "forwardJump",KeyCode.Return,true,false));
        _guardianStates.Add(new State("Please press <sprite=14> to travel forwards!", "forwardJump",KeyCode.E,true,true));
        _guardianStates.Add(new State("Please press <sprite=30> to travel forwards!", "forwardJump",KeyCode.E,true,false));
        _guardianStates.Add(new State("Awesome!!It's the end of the tutorial.You are ready to play!!", "forwardJump",KeyCode.Return,false,true));
    }

    private void CreateStatesMiner()
    {
        _minerStates = new List<State>();
        _minerStates.Add(new State("Welcome to tutorial Miner!\n\nPlease use <sprite=9> keys to move around.","backJump", KeyCode.S,false,true));
        _minerStates.Add(new State("Welcome to tutorial Miner!\n\nPlease use <sprite=26> keys to move around.","backJump", KeyCode.S,false,false));
        _minerStates.Add(new State("Press <sprite=12> + <sprite=2> to sprint. ","backJump", KeyCode.W,false,true));
        _minerStates.Add(new State("Press <sprite=29> + <sprite=21> to sprint. ","backJump", KeyCode.W,false,false));
        _minerStates.Add(new State("Use <sprite=15> to jump.","backJump", KeyCode.Space,false,true));
        _minerStates.Add(new State("Use <sprite=31> to jump.","backJump", KeyCode.Space,false,false));
        _minerStates.Add(new State("Now,let's have a look at game features!!","backJump", KeyCode.A,false,false));
        _minerStates.Add(new State("This is the timer which shows the game time.\nYou have 5 minutes!!","timer", KeyCode.Return,true,false));
        _minerStates.Add(new State("This shows the team you are in!!","team", KeyCode.Return,true,false));
        _minerStates.Add(new State("This is the timebar which helps you to see where you are at in time.","timebar", KeyCode.Return,true,false));
        _minerStates.Add(new State("IT'S TIME TO TIME TRAVEL!!","timebar", KeyCode.Return,false,false));
        _minerStates.Add(new State("This icon shows the ability of time travelling backwards.\nOnce it turns to green you can travel back in time.", "backJump",KeyCode.Return,true,false));
        _minerStates.Add(new State("Now, you are ready to go to the past!!\n\nPlease press <sprite=7>.", "backJump",KeyCode.Q,true,true));
        _minerStates.Add(new State("Now, you are ready to go to the past!!\n\nPlease press <sprite=24>.", "backJump",KeyCode.Q,true,false));
        _minerStates.Add(new State("Well done! You just traveled back in time!", "timebar",KeyCode.Return,false,false));
        _minerStates.Add(new State("Let's travel forwards now!!\nThis icon shows the ability of time travelling forwards.\nOnce it turns to green you can travel forward in time.", "forwardJump",KeyCode.Return,true,false));
        _minerStates.Add(new State("Please press <sprite=14> to travel forwards!", "forwardJump",KeyCode.E,true,true));
        _minerStates.Add(new State("Please press <sprite=30> to travel forwards!", "forwardJump",KeyCode.E,true,false));
        _minerStates.Add(new State("Awesome!!It's the end of the tutorial.You are ready to play!!", "forwardJump",KeyCode.Return,false,true));
    }

    IEnumerator DelayPopup() {

        yield return new WaitForSeconds(4);
        MoveToNextState();
        _hasMovedOn = true;
    }

    private void NeedKeyPress(bool keyPressNeeded)
    {
        if (_currentState <= _states.Count)
        {
            if ((keyPressNeeded == true) && (Input.GetKeyDown(_states[_currentState].InputTrigger)))
            {
                MoveToNextState();
            }
            else if ((keyPressNeeded == false) && _hasMovedOn)
            {    
                StartCoroutine(DelayPopup());
                _hasMovedOn = false;      
            }
        }    
   }
  

    private void MoveToNextState()
    {
            _currentState++;
            _tutorialHud.SetMessage(_states[_currentState].Message);
            _tutorialHud.SetArrowPosition(_states[_currentState].ElementToPointTo);
            _tutorialHud.SetArrowVisibility(_states[_currentState].VisibilityOfArrow);
            _tutorialHud.SetCrystalVisibility(_states[_currentState].CrystalVisibility);
    }


    // ------------ PUBLIC METHODS ------------

    public void SetTeam(Constants.Team team)
    {
        if (team == Constants.Team.Guardian) _states = _guardianStates;
        else if(team == Constants.Team.Miner) _states = _minerStates;
    }

    public void StartTutorial()
    {
        _currentState = 0;
        _tutorialHud.SetMessage(_states[_currentState].Message);
        _tutorialHud.SetArrowPosition(_states[_currentState].ElementToPointTo);
        _tutorialHud.SetArrowVisibility(_states[_currentState].VisibilityOfArrow);
        _tutorialHud.SetCrystalVisibility(_states[_currentState].CrystalVisibility);
        NeedKeyPress(_states[_currentState].NeedKey);
        _tutorialHud.SetVisibility(true);
    }

    public void StopTutorial()
    {
        _tutorialHud.SetVisibility(false);
    }

    public void SkipTutorial()
    {
        _tutorialHud.SetOptionsText("Skip tutorial <sprite=3>");

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {        
            _currentState = _states.Count - 1;

            _tutorialHud.SetMessage(_states[_currentState].Message);

        }
    }

    public void StartTutorialOver()
    {    
            if(Input.GetKeyDown(KeyCode.Alpha1)) StartTutorial();
        
            if (PhotonNetwork.IsMasterClient)
            {
                _tutorialHud.SetOptionsText("Go back to tutorial <sprite=1>\n\n Or start the game <sprite=8>");
                //_tutorialHud.SetOptionsText("Go back to tutorial <sprite=1>");
            }
            
            else {

                _tutorialHud.SetOptionsText("Go back to tutorial <sprite=1>");
            }
    }
} 
