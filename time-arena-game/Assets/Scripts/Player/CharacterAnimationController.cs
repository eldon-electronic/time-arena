using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator GuardianAnim;
    [SerializeField] private Animator MinerAnim;
    [SerializeField] private Animator PlayerAnim;
    [SerializeField] private GameObject _miner;
    [SerializeField] private GameObject _guardian;
    [SerializeField] private PlayerController _player;
    [SerializeField] private PlayerGrab _grab;

    void Awake() 
    {
        SetCharacter(_player.Team);
    }
    // Start is called before the first frame update
    void Start()
    {
        //AnimationKeyControl();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationKeyControl();
    }
    public void AnimationKeyControl(){
        if(Input.GetKeyDown(KeyCode.W)) StartRunningForwards();
        if(Input.GetKeyDown(KeyCode.S)) StartRunningBackwards();
        if(Input.GetKeyUp(KeyCode.W)) StopRunningForwards();
        if(Input.GetKeyUp(KeyCode.S)) StopRunningBackwards();
        if(Input.GetKeyDown(KeyCode.Space)) PlayerAnim.Play("Jumping");
        if(Input.GetMouseButtonDown(0)) _grab.Grab();
    }
    public void StartRunningForwards(){
        PlayerAnim.SetBool("isRunningForwards",true);
    }
    public void StartRunningBackwards(){
        PlayerAnim.SetBool("isRunningBackwards",true);
    }
    public void StopRunningForwards(){
        PlayerAnim.SetBool("isRunningForwards",false);
    }
    public void StopRunningBackwards(){
        PlayerAnim.SetBool("isRunningBackwards",false);
    }
 
    public void SetCharacter(Constants.Team team){
        if(team == Constants.Team.Guardian){
            _guardian.SetActive(true);
            _miner.SetActive(false);
            SetGuardianAnimations();
        }
        else if(team == Constants.Team.Miner){
            _guardian.SetActive(false);
            _miner.SetActive(true);
            SetMinerAnimations();
        }
    }
    public void SetGuardianAnimations(){
        PlayerAnim = GuardianAnim;
    }
    public void SetMinerAnimations(){
        PlayerAnim = MinerAnim;
    }
}
