using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    class State{
        public string Message;
        public string ElementToPointTo;
        public KeyCode InputTrigger;
        public bool VisibilityOfArrow;
        public bool NeedKey;

        public State(string message, string elementToPointTo, KeyCode inputTrigger ,bool visibilityOfArrow,bool needKeyPress)
        {
            Message = message;
            ElementToPointTo = elementToPointTo;
            InputTrigger = inputTrigger;
            VisibilityOfArrow = visibilityOfArrow;
            NeedKey = needKeyPress;
        }
    }
    
    private bool _hasMovedOn = true;
    public PlayerHud TutorialHud;
    private List<State> _states;
    private int _currentState;
  

    // Start is called before the first frame update
    void Start()
    {
        //_currentState = 0;
        //TutorialHud.SetMessage(_states[_currentState].Message);
        //TutorialHud.SetArrowPosition(_states[_currentState].ElementToPointTo);
        //TutorialHud.SetArrowVisibility(_states[_currentState].VisibilityOfArrow);
        //TutorialHud.SetTutorialVisibility(true);
        //NeedKeyPress(_states[_currentState].NeedKey);
        
       
       
        
    }

    // Update is called once per frame
    void Update()
    {
         
        UpdateTutorialOptions();
           
    }
    private void CreateStatesGuardian(){

            _states = new List<State>();
            State state1 = new State("Welcome to tutorial Guardian!\n\nPlease use <sprite=9> keys to move around.","backJump", KeyCode.S,false,true);
            _states.Add(state1);
            State state2 = new State("Welcome to tutorial Guardian!\n\nPlease use <sprite=26> keys to move around.","backJump", KeyCode.S,false,false);
            _states.Add(state2);
            State state3 = new State("Press <sprite=12> + <sprite=2> to sprint. ","backJump", KeyCode.W,false,true);
            _states.Add(state3);
            State state4 = new State("Press <sprite=29> + <sprite=21> to sprint. ","backJump", KeyCode.W,false,false);
            _states.Add(state4);
            State state5 = new State("Use <sprite=15> to jump.","backJump", KeyCode.Space,false,true);
            _states.Add(state5);
            State state6 = new State("Use <sprite=31> to jump.","backJump", KeyCode.Space,false,false);
            _states.Add(state6);
            State state7 = new State("Click right <sprite=13> to grab miners.","backJump", KeyCode.Mouse0,false,true);
            _states.Add(state7);
            State state8 = new State("Click right <sprite=23> to grab miners.","backJump", KeyCode.Space,false,false);
            _states.Add(state8);
            State state9 = new State("Now,let's have a look at game features!!","backJump", KeyCode.A,false,false);
            _states.Add(state9);
            State state10 = new State("This is the timer which shows the game time.\nYou have 5 minutes!!","timer", KeyCode.Return,true,false);
            _states.Add(state10);
            State state11 = new State("This shows the team you are in!!","team", KeyCode.Return,true,false);
            _states.Add(state11);
            State state12 = new State("This is the timebar which helps you to see where you are at in time.","timebar", KeyCode.Return,true,false);
            _states.Add(state12);
            State state13 = new State("IT'S TIME TO TIME TRAVEL!!","timebar", KeyCode.Return,false,false);
            _states.Add(state13);
            State state14 = new State("This icon shows the ability of time travelling backwards.\nOnce it turns to green you can travel back in time.", "backJump",KeyCode.Return,true,false);
            _states.Add(state14);
            State state15 = new State("Now, you are ready to go to the past!!\n\nPlease press <sprite=7>.", "backJump",KeyCode.Q,true,true);
            _states.Add(state15);
             State state16 = new State("Now, you are ready to go to the past!!\n\nPlease press <sprite=24>.", "backJump",KeyCode.Q,true,false);
            _states.Add(state16);
            State state17 = new State("Well done! You just traveled back in time!", "timebar",KeyCode.Return,false,false);
            _states.Add(state17);
            State state18 = new State("Let's travel forwards now!!\nThis icon shows the ability of time travelling forwards.\nOnce it turns to green you can travel forward in time.", "forwardJump",KeyCode.Return,true,false);
            _states.Add(state18);
            State state19 = new State("Please press <sprite=14> to travel forwards!", "forwardJump",KeyCode.E,true,true);
            _states.Add(state19);
            State state20 = new State("Please press <sprite=30> to travel forwards!", "forwardJump",KeyCode.E,true,false);
            _states.Add(state20);
            State state21 = new State("Awesome!!It's the end of the tutorial.You are ready to play!!", "forwardJump",KeyCode.Return,false,true);
            _states.Add(state21);
     
    
    }
    private void CreateStatesMiner()
    {
        _states = new List<State>();
        State state1 = new State("Welcome to tutorial Miner!\n\nPlease use <sprite=9> keys to move around.","backJump", KeyCode.S,false,true);
        _states.Add(state1);
        State state2 = new State("Welcome to tutorial Miner!\n\nPlease use <sprite=26> keys to move around.","backJump", KeyCode.S,false,false);
        _states.Add(state2);
        State state3 = new State("Press <sprite=12> + <sprite=2> to sprint. ","backJump", KeyCode.W,false,true);
        _states.Add(state3);
        State state4 = new State("Press <sprite=29> + <sprite=21> to sprint. ","backJump", KeyCode.W,false,false);
        _states.Add(state4);
        State state5 = new State("Use <sprite=15> to jump.","backJump", KeyCode.Space,false,true);
        _states.Add(state5);
        State state6 = new State("Use <sprite=31> to jump.","backJump", KeyCode.Space,false,false);
        _states.Add(state6);
        State state7 = new State("Now,let's have a look at game features!!","backJump", KeyCode.A,false,false);
        _states.Add(state7);
        State state8 = new State("This is the timer which shows the game time.\nYou have 5 minutes!!","timer", KeyCode.Return,true,false);
        _states.Add(state8);
        State state9 = new State("This shows the team you are in!!","team", KeyCode.Return,true,false);
        _states.Add(state9);
        State state10 = new State("This is the timebar which helps you to see where you are at in time.","timebar", KeyCode.Return,true,false);
        _states.Add(state10);
        State state11 = new State("IT'S TIME TO TIME TRAVEL!!","timebar", KeyCode.Return,false,false);
        _states.Add(state11);
        State state12 = new State("This icon shows the ability of time travelling backwards.\nOnce it turns to green you can travel back in time.", "backJump",KeyCode.Return,true,false);
        _states.Add(state12);
        State state13 = new State("Now, you are ready to go to the past!!\n\nPlease press <sprite=7>.", "backJump",KeyCode.Q,true,true);
        _states.Add(state13);
        State state14 = new State("Now, you are ready to go to the past!!\n\nPlease press <sprite=24>.", "backJump",KeyCode.Q,true,false);
        _states.Add(state14);
        State state15 = new State("Well done! You just traveled back in time!", "timebar",KeyCode.Return,false,false);
        _states.Add(state15);
        State state16 = new State("Let's travel forwards now!!\nThis icon shows the ability of time travelling forwards.\nOnce it turns to green you can travel forward in time.", "forwardJump",KeyCode.Return,true,false);
        _states.Add(state16);
        State state17 = new State("Please press <sprite=14> to travel forwards!", "forwardJump",KeyCode.E,true,true);
        _states.Add(state17);
        State state18 = new State("Please press <sprite=30> to travel forwards!", "forwardJump",KeyCode.E,true,false);
        _states.Add(state18);
        State state19 = new State("Awesome!!It's the end of the tutorial.You are ready to play!!", "forwardJump",KeyCode.Return,false,true);
        _states.Add(state19);
 
    }

    IEnumerator DelayPopup() {

        yield return new WaitForSeconds(4);
        MoveToNextState();
        _hasMovedOn = true;
    }

    private void NeedKeyPress(bool KeyPressNeeded){

        if(_currentState < _states.Count){
            if((KeyPressNeeded == true) && (Input.GetKeyDown(_states[_currentState].InputTrigger))){
               
                MoveToNextState();
                
            }
            else if((KeyPressNeeded == false) && _hasMovedOn){
                
                StartCoroutine(DelayPopup());
                _hasMovedOn = false;
                   
            }
            
        }
        
   }
  

    private void MoveToNextState(){

            _currentState++;
            TutorialHud.SetMessage(_states[_currentState].Message);
            TutorialHud.SetArrowPosition(_states[_currentState].ElementToPointTo);
            TutorialHud.SetArrowVisibility(_states[_currentState].VisibilityOfArrow);  
           
    }


    // ------------ PUBLIC METHODS ------------

    public void SetTeam(Constants.Team team)
    {
        
        if(team == Constants.Team.Guardian){
            CreateStatesGuardian();
        }
        else if(team == Constants.Team.Miner){
            CreateStatesMiner();
        }
    }

    public void StartTutorial()
    {
        
        _currentState = 0;
        TutorialHud.SetMessage(_states[_currentState].Message);
        TutorialHud.SetArrowPosition(_states[_currentState].ElementToPointTo);
        TutorialHud.SetArrowVisibility(_states[_currentState].VisibilityOfArrow);
        NeedKeyPress(_states[_currentState].NeedKey);
        TutorialHud.SetTutorialVisibility(true);
        
    }

    public void StopTutorial()
    {
        TutorialHud.SetTutorialVisibility(false);
    }

    public void SkipTutorial(){

        TutorialHud.SetOptionsText("Skip tutorial <sprite=3>");

        if(Input.GetKeyDown(KeyCode.Alpha2)){
                
            _currentState = _states.Count - 1;
            TutorialHud.SetMessage(_states[_currentState].Message);

        }        
        
    }

    public void StartTutorialOver(){
        
        TutorialHud.SetOptionsText("Go back to tutorial <sprite=1>\n\n OR start the game <sprite=8>");

        if(Input.GetKeyDown(KeyCode.Alpha1)){
                
            StartTutorial();
 
        }
        
    }
    public void UpdateTutorialOptions(){

    
        if ((_currentState == (_states.Count-1))){

            StartTutorialOver();

        }
        else if(_currentState <_states.Count-1){

            SkipTutorial();

        }
       
        NeedKeyPress(_states[_currentState].NeedKey);
        

    }
   
    
} 

