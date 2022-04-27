using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrystalParent : MonoBehaviour
{
    public int startFrame;
    public bool boxRequired;
    private TimeLord _timeLord;
    BoxCollider _ObjectCollider;

    void OnEnable()
    {
        GameController.newTimeLord += OnNewTimeLord;
    }
    
    void OnDisable()
    {
        GameController.newTimeLord -= OnNewTimeLord;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "PreGameScene")
        {
            _timeLord = FindObjectOfType<PreGameController>().GetComponent<PreGameController>().GetTimeLord();
        }
        else _timeLord = FindObjectOfType<GameController>().GetComponent<GameController>().GetTimeLord();
        _ObjectCollider = GetComponent<BoxCollider>();
    }

    private void OnNewTimeLord(TimeLord time)
    {
        _timeLord = time;
    } 

    void Update()
    {
        int frame = _timeLord.GetYourFrame();
        if (frame >= startFrame && _ObjectCollider.isTrigger == false) _ObjectCollider.isTrigger = true;
        else if (frame < startFrame && _ObjectCollider.isTrigger == true) _ObjectCollider.isTrigger = false;
    }
}
