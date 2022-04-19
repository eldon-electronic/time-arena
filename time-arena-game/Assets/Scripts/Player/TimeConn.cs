using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public abstract class PPController: MonoBehaviour
{
	public abstract void TriggerPP(Constants.JumpDirection direction, bool jumpOut);
}


public class TimeConn : MonoBehaviour, ParticleUser
{
	[SerializeField] private HudDebugPanel _debugPanel;
	[SerializeField] private ParticleController _particles;
	[SerializeField] private PhotonView _view;
	[SerializeField] private TailManager _tailManager;
	[SerializeField] private PPController _ppController;
	private SceneController _sceneController;
	private TimeLord _timelord;
	private bool _isJumping;
	private Constants.JumpDirection _jumpDirection;
	private bool _setJumpState;
	private float _forwardsJumpCooldown;
	private float _backJumpCooldown;
	private bool _timeTravelEnabled;
	private bool _isDissolving;


	// ------------ UNITY METHODS ------------

	void Awake()
	{
		_isJumping = false;
		_jumpDirection = Constants.JumpDirection.Static;
		_setJumpState = false;
		_forwardsJumpCooldown = 15f;
		_backJumpCooldown = 15f;
		_timeTravelEnabled = true;
		_isDissolving = false;
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
		_sceneController = FindObjectOfType<PreGameController>();
		if (_sceneController == null) Debug.LogError("PreGameController not found");
		else SetTimeLord();
		_particles.SetSubscriber(this);
	}

	void Update() {
		// Local keys only affect client's player.
		if (_view.IsMine)
		{
			UpdateCooldowns();
			KeyControl();
		}
		if (_timeTravelEnabled) UpdateTimeTravel();
	}


	// ------------ ON EVENT METHODS ------------

	private void OnGameActive(GameController game)
	{
		_sceneController = game;
		SetTimeLord();
		_timeTravelEnabled = false;
		_forwardsJumpCooldown = 15;
		_backJumpCooldown = 15;
	}

	private void OnGameStarted()
	{
		_timeTravelEnabled = true;
	}

	private void OnGameEnded(Constants.Team winningTeam)
	{
		_timeTravelEnabled = false;
		Destroy(this);
	}

	public void NotifyStartedDissolving()
	{
		_isDissolving = true;
		if (_timelord.InYourReality(_view.ViewID))
		{
			gameObject.layer = Constants.LayerPlayer;
		}
	}

	public void NotifyStoppedDissolving(bool dissolvedOut)
	{
		_isDissolving = false;
		if (dissolvedOut)
		{
			_jumpDirection = Constants.JumpDirection.Static;
			if (!_view.IsMine) gameObject.layer = Constants.LayerOutsideReality;
		}
	}


	// ------------ PRIVATE METHODS ------------

	private void SetTimeLord()
	{
		if (_view.IsMine && _timelord != null) _debugPanel.UnRegister(_timelord);
		_timelord = _sceneController.GetTimeLord();
		_timelord.Connect(_view.ViewID, _view.IsMine);
		_timelord.EnterReality(_view.ViewID);
		if (_view.IsMine) _debugPanel.Register(_timelord);
	}

	// Returns true if you can jump in the given direction.
	private bool CanTimeTravel(Constants.JumpDirection direction)
	{
		if (direction == Constants.JumpDirection.Backward)
		{
			return _backJumpCooldown <= 0f && _timelord.CanJump(_view.ViewID, Constants.JumpDirection.Backward);
		}
		else if (direction == Constants.JumpDirection.Forward)
		{
			return _forwardsJumpCooldown <= 0f && _timelord.CanJump(_view.ViewID, Constants.JumpDirection.Forward);
		}
		else
		{
			Debug.LogError("Can't jump without a direction.");
			return false;
		}
	}

	private void TimeJump(Constants.JumpDirection direction, bool jumpOut)
	{
		if (_timeTravelEnabled)
		{
			if (jumpOut)
			{
				if (CanTimeTravel(direction))
				{
					_view.RPC("RPC_jumpOut", RpcTarget.All, direction);
					_tailManager.EnableParticles(false);
					_ppController?.TriggerPP(direction, jumpOut);
				}
			}
			else if (_isJumping)
			{
				int frame = _timelord.GetNearestReality(_view.ViewID);
				_view.RPC("RPC_jumpIn", RpcTarget.All, _view.ViewID, frame);
				_tailManager.EnableParticles(true);
				_ppController?.TriggerPP(direction, jumpOut);
			}
		}
	}

	private void UpdateCooldowns()
	{
		_forwardsJumpCooldown = (_forwardsJumpCooldown > 0) ? (_forwardsJumpCooldown - Time.deltaTime) : 0;
		_backJumpCooldown = (_backJumpCooldown > 0) ? (_backJumpCooldown - Time.deltaTime) : 0;
	}

	private void KeyControl()
	{
		if (Input.GetKeyDown(KeyCode.Q)) TimeJump(Constants.JumpDirection.Backward, true);
		if (Input.GetKeyDown(KeyCode.E)) TimeJump(Constants.JumpDirection.Forward, true);
		if (Input.GetKeyUp(KeyCode.Q)) TimeJump(Constants.JumpDirection.Backward, false);
		if (Input.GetKeyUp(KeyCode.E)) TimeJump(Constants.JumpDirection.Forward, false);
		if (Input.GetKeyDown(KeyCode.L)) _timelord.SnapshotStates("GameSnapshot.txt");
	}


	private void UpdateTimeTravel()
	{
		// Record your state in all realities you exist in.
		Vector3 pos = transform.position;
		Quaternion rot = transform.rotation;
		Constants.JumpDirection dir = Constants.JumpDirection.Static;

		// Only set the jump direction once, when the _setJumpState flag is active.
		if (_setJumpState)
		{
			dir = _jumpDirection;
			_setJumpState = false;
		}

		PlayerState ps = new PlayerState(_view.ViewID, pos, rot, dir, _isJumping);
		_timelord.RecordState(ps);

		if (_isJumping)
		{
			// Perform the time jump.
			if (_timelord.CanJump(_view.ViewID, _jumpDirection))
			{
				_timelord.TimeTravel(_view.ViewID, _jumpDirection);
			}
			// Force stop jumping.
			else TimeJump(_jumpDirection, false);
		}
		else _jumpDirection = Constants.JumpDirection.Static;
	}


	// ------------ RPC FUNCTIONS ------------

	[PunRPC]
	void RPC_jumpOut(Constants.JumpDirection direction)
	{
		_isJumping = true;
		_jumpDirection = direction;
		_setJumpState = true;
		_timelord.LeaveReality(_view.ViewID);
		if (direction == Constants.JumpDirection.Forward) _forwardsJumpCooldown = 15;
		else _backJumpCooldown = 15;

		if (_view.IsMine) _sceneController.HideAllPlayers();
		else if (!_view.IsMine && gameObject.layer == Constants.LayerPlayer)
		{
			_particles.StartDissolving(_jumpDirection, true);
		}
	}

	[PunRPC]
	void RPC_jumpIn(int playerID, int frame)
	{
		_isJumping = false;
		_timelord.SetPerceivedFrame(playerID, frame);
		_timelord.EnterReality(_view.ViewID);
		
		if (_view.IsMine)
		{
			// TODO: The following line might be redundant?
			gameObject.layer = Constants.LayerPlayer;
			_sceneController.ShowPlayersInReality();
		}
		else if (_timelord.InYourReality(_view.ViewID))
		{
			_particles.StartDissolving(_jumpDirection, false);
		}
	}


	// ------------ PUBLIC METHODS ------------

	public (float forward, float back) GetCooldowns() { return (_forwardsJumpCooldown, _backJumpCooldown); }

	public (bool forward, bool back) GetCanJump()
	{
		bool canJumpForward = _timeTravelEnabled && CanTimeTravel(Constants.JumpDirection.Forward);
		bool canJumpBack = _timeTravelEnabled && CanTimeTravel(Constants.JumpDirection.Backward);
		return (canJumpForward, canJumpBack);
	}
}
