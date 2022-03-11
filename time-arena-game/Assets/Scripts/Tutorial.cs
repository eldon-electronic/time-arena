using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static System.Collections.IEnumerable;

public class Tutorial : MonoBehaviour
{
    public PlayerHud TutorialHud;
    
    class State{
        public string Message;
        public string ElementToPointTo;
        public KeyCode InputTrigger;
        public bool _visibilityOfArrow;
    

        public State(string message, string elementToPointTo, KeyCode inputTrigger ,bool visibilityOfArrow)
        {
            Message = message;
            ElementToPointTo = elementToPointTo;
            InputTrigger = inputTrigger;
            _visibilityOfArrow = visibilityOfArrow;

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
        TutorialHud.SetArrowVisibility(_states[_currentState]._visibilityOfArrow);  
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_currentState < _states.Count){
            
            if(Input.GetKeyDown(_states[_currentState].InputTrigger))
            {
                _currentState++;
                TutorialHud.SetMessage(_states[_currentState].Message);
                TutorialHud.SetArrowPosition(_states[_currentState].ElementToPointTo);
                TutorialHud.SetArrowVisibility(_states[_currentState]._visibilityOfArrow);                
            }
            

        }
       
    
       
        
    }

    private void CreateStates(){

        State state1 = new State("Welcome to Tutorial!\nPlease press W to jump.","backJump", KeyCode.W,false);
        _states.Add(state1);
        State state2 = new State("Good Job!\nPlease press A to move left.","backJump", KeyCode.A,false);
        _states.Add(state2);
        State state3 = new State("Good Job!\nPlease press D to move right.", "forwardJump", KeyCode.D,false);
        _states.Add(state3);
        State state4 = new State("Good Job!\nPlease press D to move right.", "forwardJump", KeyCode.D,false);
        _states.Add(state4);
       // State state1 = new State("Press 1 to go back in time", "backJump", KeyCode.Alpha1);
      //  _states.Add(state1);
       // State state1 = new State("Press 1 to go back in time", "backJump", KeyCode.Alpha1);
       // _states.Add(state1);
    
        
    }
    
} 

