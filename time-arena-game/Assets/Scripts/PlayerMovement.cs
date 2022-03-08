using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public GameController Game;
    public PhotonView View;
    public CharacterController CharacterBody;
    public PauseManager PauseUI;
    public Transform GroundCheck;
    public LayerMask GroundMask;
    public GameObject CameraHolder;

    private float _speed;
    private float _groundCheckRadius;
    private Vector3 _velocity;
    private float _jumpPower;
    private float _gravity;
    private bool _isGrounded;
    private float _xRot;
    private float _mouseSensitivity;

    void Start()
    {
        _speed = 5f;
        _groundCheckRadius = 0.5f;
        _jumpPower = 3f;
        _gravity = 40f;
        _isGrounded = true;
        _xRot = 0f;
        _mouseSensitivity = 100f;

        SceneManager.activeSceneChanged += onSceneChange;
    }

    void onSceneChange(Scene current, Scene next) {
		if (next.name == "GameScene") {
			Game = FindObjectOfType<GameController>();
			if (Game == null) {
				Debug.Log("Scene change error: GameController is null");
			}
		}
	}

    private void UpdatePosition()
    {
		if (!(SceneManager.GetActiveScene().name == "GameScene" && Game.gameStarted)) return;

        // Sprint speed.
        if (Input.GetKey("left shift")) _speed = 10f;
		else _speed = 5f;

        // Get movement axis values.
        float xMove = PauseUI.isPaused ? 0 : Input.GetAxis("Horizontal");
        float zMove = PauseUI.isPaused ? 0 : Input.GetAxis("Vertical");

        // Check if player's GroundCheck intersects with any environment object.
        bool isGrounded = Physics.CheckSphere(GroundCheck.position, _groundCheckRadius, GroundMask);

        // Set and normalise movement vector.
        Vector3 movement = (transform.right * xMove) + (transform.forward * zMove);
        if (movement.magnitude != 1 && movement.magnitude != 0)
        {
            movement /= movement.magnitude;
        }
        
        // Transform according to movement vector.
        CharacterBody.Move(movement * _speed * Time.deltaTime);

		// Jump control.
		if (Input.GetButtonDown("Jump") && _isGrounded && !PauseUI.isPaused)
        {
			_velocity.y += Mathf.Sqrt(_jumpPower * 2f * _gravity);
		}

		// Gravity effect.
		_velocity.y -= _gravity * Time.deltaTime;
		if (_velocity.y <= -100f) _velocity.y = -100f;

		// Reset vertical velocity value when grounded.
		if (_isGrounded && _velocity.y < 0) _velocity.y = 0f;

		// Move player according to gravity.
		CharacterBody.Move(_velocity * Time.deltaTime);
    }

    private void UpdateRotation()
    {
        // Rotate player about y and playercam about x.
		// Get axis values from input.
        // deltaTime used for fps correction.
		float mouseX = PauseUI.isPaused ? 0 : Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
		float mouseY = PauseUI.isPaused ? 0 : Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

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
        if (!View.IsMine) return;

        if (SceneManager.GetActiveScene().name == "PreGameScene" ||
		(SceneManager.GetActiveScene().name == "GameScene" && !Game.gameEnded))
        {
            UpdatePosition();
            UpdateRotation();
        }
    }
}
