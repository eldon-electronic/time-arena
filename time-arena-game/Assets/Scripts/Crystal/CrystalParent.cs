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
    private TimeConn _playerTime;
    private bool _activeHints;
    BoxCollider _ObjectCollider;

    void Awake() { _activeHints = true; }

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

        // Enable/disable hints.
        if (Input.GetKeyDown(Constants.KeyToggleHints)) { _activeHints = !_activeHints; }

        // Set name tag visibility.
        if (_activeHints && _playerTransform != null)
        {
            int currentFrame = _timeLord.GetCurrentFrame();
            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            _nameTag.SetActive(_playerTime.CanTimeTravel(Constants.JumpDirection.Forward) &&
                            yourFrame < startFrame && startFrame < currentFrame && 
                            2 < distance && distance < 10);
        }
        else _nameTag.SetActive(false);
    }

    private void OnNewTimeLord(TimeLord time) { _timeLord = time; }

    private void OnClientEntered(PlayerController pc)
    {
        _playerTransform = pc.gameObject.transform;
        _playerTime = pc.gameObject.GetComponent<TimeConn>();
    }
}
