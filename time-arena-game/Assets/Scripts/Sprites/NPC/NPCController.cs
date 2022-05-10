using UnityEngine;

public class NPCController : MonoBehaviour, DissolveUser
{
    [SerializeField] private GameObject _nameTag;
    [SerializeField] private DissolveController _dissolveController;
    [SerializeField] private ParticleController _particleController;
    private Transform _playerTransform;
    private bool _activeHints;


    // ------------ UNITY METHODS ------------

    void Awake() { _activeHints = true; }

    void OnEnable() { PlayerController.clientEntered += OnClientEntered; }
    
    void OnDisable() { PlayerController.clientEntered -= OnClientEntered; }

    void Start() { _dissolveController.SetSubscriber(this); }

    void Update()
    {
        // Enable/disable hints.
        if (Input.GetKeyDown(Constants.KeyToggleHints)) { _activeHints = !_activeHints; }

        // Set name tag visibility.
        if (_activeHints && _playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            _nameTag.SetActive(2 < distance && distance < 10);
        }
        else _nameTag.SetActive(false);
    }


    // ------------ OTHER METHODS ------------

    private void OnClientEntered(PlayerController pc) { _playerTransform = pc.gameObject.transform; }

    public void GetGrabbed()
    {
        _dissolveController.TriggerDissolve(Constants.JumpDirection.Backward, true);
        _particleController.StartParticles(Constants.JumpDirection.Backward);
    }

    public void NotifyStartedDissolving() { }

    public void NotifyStoppedDissolving(bool dissolvedOut)
	{
        Destroy(gameObject);
        Destroy(this);
	}
}
