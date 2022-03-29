using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, ParticleUser
{

	// Variables defining player values.
	public Camera Cam;
	public GameObject CameraHolder;
	public PlayerMaterial Material;
	public PlayerMovement Movement;

	// Variables corresponding to UI.
	public Canvas UI;
	public PauseManager PauseUI;
	public GameObject Nametag;
	public PlayerHud Hud;
	public Tutorial Tutorial;

    // Variables corresponding to player Animations.
	public Animator PlayerAnim;
	public Transform GrabCheck;
	public LayerMask GrabMask;
	private float _grabCheckRadius = 1f;
	private bool _damageWindow = false;
	public ParticleController Particles;

    // The photonView component that syncs with the network.
	public PhotonView View;

	// Time control variables.
	private bool _isJumping;
	private Constants.JumpDirection _jumpDirection;
	private bool _dissolvedOut;

    // Variables corresponding to the gamestate.
	private PreGameController _preGame;
    private GameController _game;
	private TimeLord _timelord;
	[SerializeField] private TailManager _tailManager;

	public Constants.Team Team;
	private float _forwardsJumpCooldown = 15f;
	private float _backJumpCooldown = 15f;
	private Vector3[] _hiderSpawnPoints;
	private Vector3 _seekerSpawnPoint;


	void Start() {
		DontDestroyOnLoad(this.gameObject);
		Team = Constants.Team.Miner;
		Material.SetMaterialMiner();
		Hud.SetTeam("HIDER");
		Tutorial.SetTeam(Constants.Team.Miner);
		Tutorial.StartTutorial();
		Particles.Subscribe(this);

		_preGame = FindObjectOfType<PreGameController>();
		if (_preGame == null) Debug.LogError("PreGameController not found");
		else _preGame.Register(this);
		
		if (!View.IsMine)
		{
			Destroy(Cam.gameObject);
			Destroy(UI.gameObject);
			gameObject.layer = 7;
		}
		else
		{
			Destroy(Nametag);
			gameObject.tag = "Client";
		}
		// Allow master client to move players from one scene to another.
        PhotonNetwork.AutomaticallySyncScene = true;
		// Lock players cursor to center screen.
        Cursor.lockState = CursorLockMode.Locked;
		// Link scenechange event to Onscenechange.
        SceneManager.activeSceneChanged += OnSceneChange;

		_hiderSpawnPoints =  new Vector3[] {
			new Vector3(-42f, 0f, 22f), 
			new Vector3(-15f, -0.5f, -4f), 
			new Vector3(-12f, -0.5f, -40f), 
			new Vector3(-47f, -0.5f, -8f), 
			new Vector3(-36f, -2.5f, 2.2f)
		};

		_seekerSpawnPoint = new Vector3(-36f, -2f, -29f);

		_isJumping = false;
		_jumpDirection = Constants.JumpDirection.Static;
	}


	private void MoveToSpawnPoint()
	{
		if (Team == Constants.Team.Miner)
		{
			int index = Random.Range(0, _hiderSpawnPoints.Length);
			Vector3 position = _hiderSpawnPoints[index];
			Movement.MoveTo(position);
		}
		else
		{
			Movement.MoveTo(_seekerSpawnPoint);
		}
	}


	// OnSceneChange is called by the SceneManager.activeSceneChanged event.
	void OnSceneChange(Scene current, Scene next)
	{
		if (next.name == "GameScene")
		{
			if (View.IsMine)
			{
				_preGame.Kill();
				_tailManager.DestroyTails();
				_tailManager.Deactivate();
			}

			_game = FindObjectOfType<GameController>();
			if (_game == null) Debug.LogError("GameController not found");
			else _game.Register(View.ViewID, this, Team);

			if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.Players.Count > 1)
			{
				// Become the Guardian.
				GetFound();
			}

			_forwardsJumpCooldown = 15;
			_backJumpCooldown = 15;

			MoveToSpawnPoint();
			Material.SetArmActive(Team == Constants.Team.Guardian);

			Tutorial.StopTutorial();
		}
	}


	// ------------ RPC FUNCTIONS ------------

	[PunRPC]
	void RPC_jumpBackOut()
	{
		_isJumping = true;
		_jumpDirection = Constants.JumpDirection.Backward;
		Particles.StartDissolving(_jumpDirection, true);
		_backJumpCooldown = 15;
		Hud.PressForwardJumpButton();
	}

	[PunRPC]
	void RPC_jumpForwardOut()
	{
		_isJumping = true;
		_jumpDirection = Constants.JumpDirection.Forward;
		Particles.StartDissolving(_jumpDirection, true);
		_forwardsJumpCooldown = 15;
		Hud.PressBackJumpButton();
	}

	[PunRPC]
	void RPC_jumpBackIn()
	{
		_isJumping = false;
		_jumpDirection = Constants.JumpDirection.Backward;
		Particles.StartDissolving(_jumpDirection, false);
		_timelord.EnterReality(View.ViewID);
	}

	[PunRPC]
	void RPC_jumpForwardIn()
	{
		_isJumping = false;
		_jumpDirection = Constants.JumpDirection.Forward;
		Particles.StartDissolving(_jumpDirection, false);
		_timelord.EnterReality(View.ViewID);
	}

	// RPC function to be called when another player finds this one.
	[PunRPC]
	void RPC_getFound() { ChangeTeam(); }

	// RPC function to be called by other machines to set this players transform.
	[PunRPC]
	void RPC_movePlayer(Vector3 pos, Vector3 rot)
	{
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
		CameraHolder.transform.rotation = Quaternion.Euler(rot);
	}


	// ------------ HELPER CONDITION CHECKERS ------------

	// Returns true if you're in a scene that allows time travelling.
	private bool TimeTravelEnabled()
	{
		return SceneManager.GetActiveScene().name == "PreGameScene" ||
			SceneManager.GetActiveScene().name == "GameScene" && _game.GameStarted && !_game.GameEnded;
	}

	// Returns true if you can jump in the given direction.
	private bool CanTimeTravel(Constants.JumpDirection direction)
	{
		if (direction == Constants.JumpDirection.Backward)
		{
			return _backJumpCooldown <= 0f && _timelord.CanJump(View.ViewID, Constants.JumpDirection.Backward);
		}
		else if (direction == Constants.JumpDirection.Forward)
		{
			return _forwardsJumpCooldown <= 0f && _timelord.CanJump(View.ViewID, Constants.JumpDirection.Forward);
		}
		else
		{
			Debug.LogError("Can't jump without a direction.");
			return false;
		}
	}


	// ------------ ACTIONS ------------

	public void JumpBackwards(bool jumpOut)
	{
		if (TimeTravelEnabled())
		{
			if (jumpOut)
			{
				if (CanTimeTravel(Constants.JumpDirection.Backward) && !Particles.IsDissolving())
				{
					View.RPC("RPC_jumpBackOut", RpcTarget.All);
					if (View.IsMine) _tailManager.DestroyTails();
				}
			}
			else
			{
				View.RPC("RPC_jumpBackIn", RpcTarget.All);
				if (View.IsMine) _tailManager.BirthTails();
			}
		}
	}

	public void JumpForward(bool jumpOut)
	{
		if (TimeTravelEnabled())
		{
			if (jumpOut)
			{
				if (CanTimeTravel(Constants.JumpDirection.Forward) && !Particles.IsDissolving())
				{
					View.RPC("RPC_jumpForwardOut", RpcTarget.All);
					if (View.IsMine) _tailManager.DestroyTails();
				}
			}
			else
			{
				View.RPC("RPC_jumpForwardIn", RpcTarget.All);
				if (View.IsMine) _tailManager.BirthTails();
			}
		}
	}

	private void Grab()
	{
		// If grabbing, check for intersection with player.
		if (!_damageWindow)
		{
			Collider[] playersGrab = Physics.OverlapSphere(GrabCheck.position, _grabCheckRadius, GrabMask);
			foreach (var playerGotGrab in playersGrab)
			{
				// Call grabplayer function on that player.
				PlayerController targetPlayer = playerGotGrab.GetComponent<PlayerController>();
				if (Team == Constants.Team.Guardian && 
					targetPlayer.Team == Constants.Team.Miner)
				{
					targetPlayer.GetFound();
				}
			}
			PlayerAnim.SetBool("isGrabbing", true);
		}
	}

	private void StartGame()
	{
		if (SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient)
		{
			Hud.StartCountingDown();
		}
	}

	public void ChangeTeam()
	{
		if (Team == Constants.Team.Miner)
		{
			Team = Constants.Team.Guardian;
			Material.SetArmActive(true);
			Hud.SetTeam("SEEKER");
		}
		else
		{
			Team = Constants.Team.Miner;
			Material.SetArmActive(false);
			Hud.SetTeam("HIDER");
		}
		Material.SetMaterial(Team);
		_game.SetTeam(View.ViewID, Team);
	}

	// RPC function to be called when another player hits this one.
	// Function to get found by calling RPC on all machines.
	public void GetFound()
	{
		View.RPC("RPC_getFound", RpcTarget.All);
	}

	// Function to move this player by calling RPC for all others.
	public void MovePlayer(Vector3 pos, Vector3 rot)
	{
		View.RPC("RPC_movePlayer", RpcTarget.All, pos, rot);
	}

	// Function to enable player to grab others.
	public void StartGrabbing()
	{
		_damageWindow = true;
	}

	// Function to disable player to grab others.
	public void StopGrabbing()
	{
		_damageWindow = false;
		PlayerAnim.SetBool("isGrabbing", false);
	}


	// ------------ UPDATE HELPER FUNCTIONS ------------

	private void UpdateCooldowns()
	{
		_forwardsJumpCooldown = (_forwardsJumpCooldown > 0) ? (_forwardsJumpCooldown - Time.deltaTime) : 0;
		_backJumpCooldown = (_backJumpCooldown > 0) ? (_backJumpCooldown - Time.deltaTime) : 0;
		float forwardBarHeight = 1.0f - (_forwardsJumpCooldown / 15.0f);
		float backBarHeight = 1.0f - (_backJumpCooldown / 15.0f);
		float[] cooldownValues = new float[]{forwardBarHeight, backBarHeight};
		Hud.SetCooldownValues(cooldownValues);

		bool canJumpForward = TimeTravelEnabled() && CanTimeTravel(Constants.JumpDirection.Forward);
		bool canJumpBack = TimeTravelEnabled() && CanTimeTravel(Constants.JumpDirection.Backward);
		Hud.SetCanJump(canJumpForward, canJumpBack);
	}

	private void UpdateTimeline()
	{
		float yourPos = _timelord.GetYourPosition();
		List<float> players = _timelord.GetPlayerPositions();
		Hud.SetPlayerPositions(yourPos, players);

		float time = _timelord.GetTimeProportion();
		Hud.SetTimeBarPosition(time);
	}

	private void UpdateTimer()
	{
		int time = _timelord.GetElapsedTime();
		Hud.SetTime(time);
	}

	private void UpdateDebugDisplay()
	{
		Hashtable debugItems = new Hashtable();
		debugItems.Add("Room", PhotonNetwork.CurrentRoom.Name);
		debugItems.Add("Sprint", Input.GetKey("left shift"));
		debugItems.Add("Grab", _damageWindow);
		debugItems.Add("My Time", _timelord.GetYourPosition());
		debugItems.Add("The Time", _timelord.GetTimeProportion());

		Hashtable movementState = Movement.GetState();
		Utilities.Union(ref debugItems, movementState);
		
		Hud.SetDebugValues(debugItems);
	}

	void KeyControl()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			Debug.Log("Key down 1");
			JumpBackwards(true);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2)) JumpForward(true);

		if (Input.GetKeyUp(KeyCode.Alpha1)) {
			Debug.Log("Key up 1");
			JumpBackwards(false);
		}

		if (Input.GetKeyUp(KeyCode.Alpha2)) JumpForward(false);

		if (Input.GetMouseButtonDown(0)) Grab();

		if (Input.GetKeyDown(KeyCode.E)) StartGame();

		if (Input.GetKeyDown(KeyCode.Escape)) Hud.StopCountingDown();

		if (Input.GetKeyDown(KeyCode.P)) Hud.ToggleDebug();
	}

	void Update() {
		// Local keys only affect client's player.
		if (!View.IsMine) return;

		if (SceneManager.GetActiveScene().name == "PreGameScene" ||
			(SceneManager.GetActiveScene().name == "GameScene" && !_game.GameEnded)) {
			UpdateCooldowns();
			UpdateTimeline();
			UpdateTimer();
			UpdateDebugDisplay();
			KeyControl();
		}

		if (TimeTravelEnabled())
		{
			// Record your state in all realities you exist in.
			Vector3 pos = Movement.GetPosition();
			Quaternion rot = Movement.GetRotation();
			PlayerState ps = new PlayerState(View.ViewID, pos, rot, _jumpDirection);
			_timelord.RecordState(ps);

			// Time travel.
			if (_isJumping) _timelord.TimeTravel(View.ViewID, _jumpDirection);
		}

		// Update pauseUI and cursor lock if game is ended.
		if (SceneManager.GetActiveScene().name == "GameScene" && _game.GameEnded)
		{
			PauseUI.Pause();
		}
	}


	// ------------ PUBLIC METHODS ------------

	public void NotifyStoppedDissolving(bool dissolvedOut)
	{
		if (dissolvedOut) _timelord.LeaveReality(View.ViewID);
		else _jumpDirection = Constants.JumpDirection.Static;
	}

	public void SetTimeLord(TimeLord timelord)
	{
		_timelord = timelord;
		_timelord.Connect(View.ViewID, View.IsMine);
		_timelord.EnterReality(View.ViewID);
		if (View.IsMine)
		{
			_tailManager.SetTimeLord(_timelord);
		}
	}
}
