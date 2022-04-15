using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour, Debuggable
{
    [SerializeField] private HudDebugPanel _debugPanel;
    public PhotonView View;
    public CharacterController CharacterBody;
    public PauseManager PauseUI;
    public Transform PlayerTransform;
    public LayerMask GroundMask;
    public GameObject CameraHolder;

    private float _speed;
    private float _groundCheckRadius;
    private Vector3 _velocity;
    private float _jumpPower;
    private float _gravity;
    private bool _isGrounded;
    private bool _isCeiling;
    private float _xRot;
    private float _mouseSensitivity;
    private bool _activated;

    void Awake()
    {
        _speed = 5f;
        _groundCheckRadius = 0.5f;
        _jumpPower = 3f;
        _gravity = 40f;
        _isGrounded = true;
        _isCeiling = false;
        _xRot = 0f;
        _mouseSensitivity = 100f;
        _activated = true;
    }

    void OnEnable()
    {
        GameController.gameActive += OnGameActive;
        GameController.gameStarted += OnGameStarted;
        GameController.gameEnded += OnGameEnded;
    }

    void OnDisable()
    {
        GameController.gameActive -= OnGameActive;
        GameController.gameStarted -= OnGameStarted;
        GameController.gameEnded -= OnGameEnded;
    }

    void Start()
    {
        Physics.IgnoreLayerCollision(Constants.LayerOutsideReality, Constants.LayerPlayer);
        Physics.IgnoreLayerCollision(Constants.LayerOutsideReality, Constants.LayerOutsideReality);

        _debugPanel.Register(this);
    }

    private void OnGameActive(GameController game) { _activated = false; }

    private void OnGameStarted() { _activated = true; }

    private void OnGameEnded() { _activated = false; }
    
    public void OnMouseSensChange(float a){ _mouseSensitivity = PauseUI.MouseSens; }

    private void UpdatePosition()
    {
        // Sprint speed.
        if (Input.GetKey("left shift") && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))) _speed = 10f;
		else _speed = 5f;

        // Get movement axis values.
        float xMove = PauseUI.IsPaused() ? 0 : Input.GetAxis("Horizontal");
        float zMove = PauseUI.IsPaused() ? 0 : Input.GetAxis("Vertical");

        // Check if player's bottom intersects with any environment object.
        Vector3 groundCheck = PlayerTransform.position;
        groundCheck.y -= 1f;
        _isGrounded = Physics.CheckSphere(groundCheck, _groundCheckRadius, GroundMask);

        //Check if player's head intersects with any environment object.
        Vector3 ceilingCheck = PlayerTransform.position;
        ceilingCheck.y += 0.6f;
        _isCeiling = Physics.CheckSphere(ceilingCheck, _groundCheckRadius, GroundMask);

        // Set and normalise movement vector.
        Vector3 movement = (transform.right * xMove) + (transform.forward * zMove);
        if (movement.magnitude != 1 && movement.magnitude != 0)
        {
            movement /= movement.magnitude;
        }

        // Transform according to movement vector.
        CharacterBody.Move(movement * _speed * Time.deltaTime);

		// Jump control.
		if (Input.GetButtonDown("Jump") && _isGrounded && !PauseUI.IsPaused())
        {
			_velocity.y += Mathf.Sqrt(_jumpPower * 2f * _gravity);
		}


        // Gravity effect.
        _velocity.y -= _gravity * Time.deltaTime;
		if (_velocity.y <= -100f) _velocity.y = -100f;

		// Reset vertical velocity value when grounded.
		if (_isGrounded && _velocity.y < 0) _velocity.y = 0f;

        // Reset vertical velocity when head it hitting ceiling.
        if (_isCeiling && _velocity.y > 0) _velocity.y = 0f;

        // Move player according to gravity.
        CharacterBody.Move(_velocity * Time.deltaTime);
    }

    private void UpdateRotation()
    {
        // Rotate player about y and playercam about x.
		// Get axis values from input.
        // deltaTime used for fps correction.
		float mouseX = PauseUI.IsPaused() ? 0 : Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
		float mouseY = PauseUI.IsPaused() ? 0 : Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

		// Invert vertical rotation and restrict up/down.
		_xRot -= mouseY;
		_xRot = Mathf.Clamp(_xRot, -90f, 90f);

		// Apply rotation.
		CameraHolder.transform.localRotation = Quaternion.Euler(_xRot, 0f, 0f);

        // Rotate player about y axis with mouseX movement.
		transform.Rotate(Vector3.up * mouseX);
    }

    void Update()
    {
        if (View.IsMine && _activated)
        {
            UpdatePosition();
            UpdateRotation();
        }
    }

    public void MoveTo(Vector3 position)
    {
        PlayerTransform.position = position;
    }

    public Vector3 GetPosition() { return PlayerTransform.position; }

    public Quaternion GetRotation() { return PlayerTransform.rotation; }

    public Hashtable GetDebugValues()
    {
        Hashtable debugValues = new Hashtable();
        debugValues.Add("IsGrounded", _isGrounded);
        return debugValues;
    }
}
