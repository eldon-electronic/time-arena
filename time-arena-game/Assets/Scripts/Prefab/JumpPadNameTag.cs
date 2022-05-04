using UnityEngine;

public class JumpPadNameTag : MonoBehaviour
{
    [SerializeField] private GameObject _nameTag;
    private Transform _playerTransform;
    private bool _active;

    void Awake() { _active = true; }

    void OnEnable() { PlayerController.clientEntered += OnClientEntered; }
    
    void OnDisable() { PlayerController.clientEntered -= OnClientEntered; }

    void Update()
    {
        if (Input.GetKeyDown(Constants.KeyToggleHints)) { _active = !_active; }

        if (_active && _playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            _nameTag.SetActive(distance > 2 && distance < 10);
        }
        else _nameTag.SetActive(false);
    }

    private void OnClientEntered(PlayerController pc) { _playerTransform = pc.gameObject.transform; }
}
