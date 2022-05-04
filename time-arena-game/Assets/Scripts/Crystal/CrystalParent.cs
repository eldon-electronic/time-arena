using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrystalParent : MonoBehaviour
{
    [SerializeField] private GameObject _nameTag;
    public int startFrame;
    public bool boxRequired;
    private TimeLord _timeLord;
    private Transform _playerTransform;
    BoxCollider _ObjectCollider;

    void OnEnable()
    {
        GameController.newTimeLord += OnNewTimeLord;
        PlayerController.clientEntered += OnClientEntered;
    }
    
    void OnDisable()
    {
        GameController.newTimeLord -= OnNewTimeLord;
        PlayerController.clientEntered -= OnClientEntered;
    }

    void Start()
    {
        _timeLord = FindObjectOfType<SceneController>().GetTimeLord();
        _ObjectCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        // Set collider trigger depending on whether in broken state.
        int yourFrame = _timeLord.GetYourPerceivedFrame();
        if (yourFrame >= startFrame && _ObjectCollider.isTrigger == false) _ObjectCollider.isTrigger = true;
        else if (yourFrame < startFrame && _ObjectCollider.isTrigger == true) _ObjectCollider.isTrigger = false;

        // Set name tag visibility.
        int currentFrame = _timeLord.GetCurrentFrame();
        float distance = Vector3.Distance(transform.position, _playerTransform.position);
        _nameTag.SetActive(yourFrame < startFrame && 
                           startFrame < currentFrame && 
                           2 < distance &&
                           distance < 10);
    }

    private void OnNewTimeLord(TimeLord time) { _timeLord = time; }

    private void OnClientEntered(PlayerController pc) { _playerTransform = pc.gameObject.transform; }
}
