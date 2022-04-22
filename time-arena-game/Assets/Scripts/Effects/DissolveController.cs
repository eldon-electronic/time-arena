using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class DissolveController : MonoBehaviour
{
    [SerializeField] private Material _minerMetal;
    [SerializeField] private Material _minerAbdomen;
    [SerializeField] private Material _minerShoe;
    [SerializeField] private Material _minerVisor;
    [SerializeField] private Material _minerEars;
    [SerializeField] private Material _guardianBody;
    [SerializeField] private Material _device;
    [SerializeField] private Material _deviceCompass;
    [SerializeField] private Material _deviceNeedle;
    [SerializeField] private Material _deviceButton;
    [SerializeField] private Material _deviceArrow;
    public Constants.Team Character;
    private float _animationDuration = 2;
    private Color _backwardColour = new Color(0, 0, 230, 0);
    private Color _forwardColour = new Color(0, 0, 230, 0);


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DissolveBackwardOut(){
        if(Character == Constants.Team.Guardian){
            
        }
        
    }
    public void DissolveForwardOut(){

    }
    public void DissolveBackwardIn(){

    }
    public void DissolveForwardIn(){

    }
}
