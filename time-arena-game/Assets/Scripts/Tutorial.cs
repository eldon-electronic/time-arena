using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public PlayerHud TutorialHud;
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
    
    private List<State> _states;
    private int _currentState;
  

    // Start is called before the first frame update
    void Start()
    {
        _states = new List<State>(); 
        _currentState = 0;
        CreateStates();
        TutorialHud.SetMessage(_states[_currentState].Message);
        TutorialHud.SetArrowPosition(_states[_currentState].ElementToPointTo);
        TutorialHud.SetArrowVisibility(_states[_currentState].VisibilityOfArrow);
        NeedKeyPress(_states[_currentState].NeedKey);
       
        
    }

    // Update is called once per frame
    void Update()
    {
         NeedKeyPress(_states[_currentState].NeedKey);
        
       
    
       // if(_currentState < _states.Count){

       
           // if((_states[_currentState].NeedKey == true)){
                //if(Input.GetKeyDown(_states[_currentState].InputTrigger)){

                   // MoveToNextState();
                    

                //}
                
            //}
            //else if(_states[_currentState].NeedKey  == false){
                //Debug.Log(_states[_currentState].NeedKey);
                //Debug.Log(_currentState);
                //MoveToNextState();
                //Invoke("MoveToNextState",4.0f);
                
                
                
            //}
            

            
           // NeedKeyPress(_states[_currentState].NeedKey);

        //}
        
       
        //if((_currentState == (_states.Count-1)) && Input.GetKeyDown(KeyCode.T))
       // {
               // _currentState = 0;
                //TutorialHud.SetMessage(_states[_currentState].Message);
                //TutorialHud.SetArrowPosition(_states[_currentState].ElementToPointTo);
                //TutorialHud.SetArrowVisibility(_states[_currentState].VisibilityOfArrow); 
                
                
               // MoveToNextState();        
                            
       // }
        //MoveToNextState();
       
    
       
        
    }
    private void CreateStates(){

        

            State state1 = new State("Welcome to Tutorial!\n\nPlease press <sprite=2> to see the next step.","backJump", KeyCode.Return,false,true);
            _states.Add(state1);
            State state2 = new State("Please use <sprite=0> keys to move.","backJump", KeyCode.Return,false,true);
            _states.Add(state2);
            State state3 = new State("Use < S to move backward.","backJump", KeyCode.S,false,true);
            _states.Add(state3);
            State state4 = new State("Good Job!\nPlease press A to move left.","backJump", KeyCode.A,false,false);
            _states.Add(state4);
            State state5 = new State("Good Job!\nPlease press D to move right.", "forwardJump", KeyCode.D,false,true);
            _states.Add(state5);
            State state6 = new State("Good Job!\nPlease press Shift to sprint while you are moving forwards.", "forwardJump", KeyCode.LeftShift,false,false);
            _states.Add(state6);
            State state7 = new State("Well Done!\nNow,let's have a look at game features.\nPress Enter to move to the next feature.", "timer", KeyCode.Return,false,true);
            _states.Add(state7);
            State state8 = new State("This is the timer which shows the game time.\nYou have 5 minutes!!","timer", KeyCode.Return,true,false);
            _states.Add(state8);
            State state9 = new State("This shows the team you are in!!","team", KeyCode.Return,true,true);
            _states.Add(state9);
            State state10 = new State("This is the timebar which helps you to see where you are at in time.","timebar", KeyCode.Return,true,false);
            _states.Add(state10);
            State state11 = new State("IT'S TIME TO TIME TRAVEL!!","timebar", KeyCode.Return,false,true);
            _states.Add(state11);
            State state12 = new State("This icon shows the ability of time travelling backwards.\nOnce it turns to green you can travel back in time.", "backJump",KeyCode.Return,true,false);
            _states.Add(state12);
            State state13 = new State("Now, you are ready to go to the past!!\nPlease press 1.", "backJump",KeyCode.Alpha1,true,true);
            _states.Add(state13);
            State state14 = new State("Well done! You just traveled back in time!", "timebar",KeyCode.Return,false,false);
            _states.Add(state14);
            State state15 = new State("Let's travel forwards now!!\nThis icon shows the ability of time travelling forwards.\nOnce it turns to green you can travel forward in time.", "forwardJump",KeyCode.Return,true,true);
            _states.Add(state15);
            State state16 = new State("Please press 2 to travel forwards!", "forwardJump",KeyCode.Alpha2,true,false);
            _states.Add(state16);
            State state17 = new State("Awesome!!It's the end of the tutorial.You are ready to play!!\nPlease press E to start the game OR press T to go back to tutorial.", "forwardJump",KeyCode.T,false,true);
            _states.Add(state17);
        
        
    }

    private void NeedKeyPress(bool KeyPressNeeded){

        if(_currentState < _states.Count){

            if((KeyPressNeeded == true) && (Input.GetKeyDown(_states[_currentState].InputTrigger))){

                _currentState++;
                MoveToNextState();
            }
            if((KeyPressNeeded != true)){
                
                
                Invoke("MoveToNextState",4f);
                MoveToNextState();
                _currentState++;
                
            }
            
            

        }

        

   }
        private void MoveToNextState(){

                 TutorialHud.SetMessage(_states[_currentState].Message);
                 TutorialHud.SetArrowPosition(_states[_currentState].ElementToPointTo);
                 TutorialHud.SetArrowVisibility(_states[_currentState].VisibilityOfArrow);  
           
    }


    // ------------ PUBLIC METHODS ------------

    public void SetTeam(Constants.Team team)
    {
        // TODO: This should update the team, changing which tutorial we follow.
    }

    public void StartTutorial()
    {
        // TODO: This should reset the tutorial to the first state and then make the tutorial active and visible.
        // (Possibly just by setting some boolean which you check for in the Update() method).
    }

    public void StopTutorial()
    {
        // TODO: This should make the tutorial inactive and switch off visibility.
    }

    
} 

