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

        public State(string message, string elementToPointTo, KeyCode inputTrigger)
        {
            Message = message;
            ElementToPointTo = elementToPointTo;
            InputTrigger = inputTrigger;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        while(_currentState < _states.Count){
            
            if(Input.GetKey(_states[_currentState].InputTrigger))
            {
                _currentState++;
            }

        }
       
        TutorialHud.SetMessage(_states[_currentState].Message);
        TutorialHud.SetArrowPosition(_states[_currentState].ElementToPointTo);
       
        
    }

    private void CreateStates(){

        State state1 = new State("Press 1 to go back in time","backJump",KeyCode.Alpha1);
        _states.Add(state1);
        State state2 = new State("Press 2 to go forward in time", "forwardJump", KeyCode.Alpha2);
        _states.Add(state2);
       // State state1 = new State("Press 1 to go back in time", "backJump", KeyCode.Alpha1);
      //  _states.Add(state1);
       // State state1 = new State("Press 1 to go back in time", "backJump", KeyCode.Alpha1);
       // _states.Add(state1);
    
        
    }
    
} 

