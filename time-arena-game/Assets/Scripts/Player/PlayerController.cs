using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, ParticleUser, Debuggable
{

	// Variables defining player values.
	public Camera Cam;
	public GameObject CameraHolder;
	public PlayerMaterial Material;
	public PlayerMovement Movement;
	[SerializeField] private CollectingCrystals _collectingCrystals;

	// Variables corresponding to UI.
	public Canvas UI;
	public PauseManager PauseUI;
	public GameObject Nametag;
	public PlayerHud Hud;
	[SerializeField] private Tutorial _tutorial;
	[SerializeField] private HudDebugPanel _debugPanel;

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
	private bool _setJumpState;

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


	void Awake()
	{
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
		_setJumpState = false;
	}

	void Start()
	{	
		DontDestroyOnLoad(this.gameObject);

		// TODO: Set the team in the menu before loading the pregame scene.
		Team = Constants.Team.Miner;
		if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.Players.Count > 1)
		{
			Team = Constants.Team.Guardian;
		}

		Material.SetMaterial(Constants.Team.Miner);
		Hud.SetTeam("MINER");
		gameObject.layer = Constants.LayerPlayer;
		Particles.Subscribe(this);

		_preGame = FindObjectOfType<PreGameController>();
		if (_preGame == null) Debug.LogError("PreGameController not found");
		else _preGame.Register(this);
		
		if (View.IsMine)
		{
			Destroy(Nametag);
			gameObject.tag = "Client";
			Hud.SetPlayer(this);
			_debugPanel.Register(this);
			_tutorial.SetTeam(Team);
			_tutorial.StartTutorial();
		}
		else
		{
			Destroy(Cam.gameObject);
			Destroy(UI.gameObject);
			gameObject.layer = 7;
		}

		// Allow master client to move players from one scene to another.
        PhotonNetwork.AutomaticallySyncScene = true;
		// Lock players cursor to center screen.
        Cursor.lockState = CursorLockMode.Locked;
		// Link scenechange event to Onscenechange.
        SceneManager.activeSceneChanged += OnSceneChange;

		// TODO: Let each player do this themselves.
		List<int> playerIDs = _timelord.GetAllPlayerIDs();
		foreach (var id in playerIDs)
		{
			PhotonView.Find(id).gameObject.layer = Constants.LayerPlayer;
		}
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
				_tailManager.SetActive(false);
				_tutorial.StopTutorial();
			}

			_forwardsJumpCooldown = 15;
			_backJumpCooldown = 15;

			MoveToSpawnPoint();
			Material.SetArmActive(Team == Constants.Team.Guardian);
		}
	}


	// ------------ RPC FUNCTIONS ------------

	[PunRPC]
	void RPC_jumpBackOut()
	{
		_isJumping = true;
		_jumpDirection = Constants.JumpDirection.Backward;
		_setJumpState = true;
		_timelord.LeaveReality(View.ViewID);
		_backJumpCooldown = 15;

		if (View.IsMine) 
		{
			if (_game == null) _preGame.HideAllPlayers();
			else _game.HideAllPlayers();
		}
		else if(!View.IsMine && gameObject.layer == Constants.LayerPlayer)
		{
			Particles.StartDissolving(_jumpDirection, true);
		}
	}

	[PunRPC]
	void RPC_jumpForwardOut()
	{
		_isJumping = true;
		_jumpDirection = Constants.JumpDirection.Forward;
		_setJumpState = true;
		_timelord.LeaveReality(View.ViewID);
		_forwardsJumpCooldown = 15;

		if (View.IsMine) 
		{
			if (_game == null) _preGame.HideAllPlayers();
			else _game.HideAllPlayers();	
		}
		else if(!View.IsMine && gameObject.layer == Constants.LayerPlayer)
		{
			Particles.StartDissolving(_jumpDirection, true);
		}
	}

	[PunRPC]
	void RPC_jumpBackIn(int playerID, int frame)
	{
		_isJumping = false;
		_timelord.SetPerceivedFrame(playerID, frame);
		_timelord.EnterReality(View.ViewID);
		
		if (View.IsMine)
		{
			// TODO: The following line might be redundant?
			gameObject.layer = Constants.LayerPlayer;
			if (_game == null) _preGame.ShowPlayersInReality();
			else _game.ShowPlayersInReality();
		}
		else
		{
			if (_timelord.InYourReality(View.ViewID))
			{
				Particles.StartDissolving(_jumpDirection, false);
			}
		}
	}

	[PunRPC]
	void RPC_jumpForwardIn(int playerID, int frame)
	{
		_isJumping = false;
		_timelord.SetPerceivedFrame(playerID, frame);
		_timelord.EnterReality(View.ViewID);

		if (View.IsMine)
		{
			// TODO: The following line might be redundant?
			gameObject.layer = Constants.LayerPlayer;
			if (_game == null) _preGame.ShowPlayersInReality();
			else _game.ShowPlayersInReality();
		}
		else
		{
			if (_timelord.InYourReality(View.ViewID))
			{
				Particles.StartDissolving(_jumpDirection, false);
			}
		}
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

	private void TimeJump(Constants.JumpDirection direction, bool jumpOut)
	{
		if (TimeTravelEnabled())
		{
			if (jumpOut)
			{
				if (CanTimeTravel(direction) && !Particles.IsDissolving())
				{
					// TODO: Start a warp post processing effect here.
					if (direction == Constants.JumpDirection.Backward)
					{
						View.RPC("RPC_jumpBackOut", RpcTarget.All);
					}
					else View.RPC("RPC_jumpForwardOut", RpcTarget.All);
					_tailManager.SetActive(true);
					_tailManager.EnableParticles(false);
				}
			}
			else if (_isJumping)
			{
				// TODO: Stop the warp post processing effect here.
				int frame = _timelord.GetNearestReality(View.ViewID);
				if (direction == Constants.JumpDirection.Backward)
				{
					View.RPC("RPC_jumpBackIn", RpcTarget.All, View.ViewID, frame);
				}
				else View.RPC("RPC_jumpForwardIn", RpcTarget.All, View.ViewID, frame);
				_tailManager.EnableParticles(true);
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
			_preGame.StartCountingDown();
		}
	}

	public void ChangeTeam()
	{
		if (Team == Constants.Team.Miner)
		{
			Team = Constants.Team.Guardian;
			Material.SetArmActive(true);
			Hud.SetTeam("GUARDIAN");
		}
		else
		{
			Team = Constants.Team.Miner;
			Material.SetArmActive(false);
			Hud.SetTeam("MINER");
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
	}

	void KeyControl()
	{
		if (Input.GetKeyDown(KeyCode.Q)) TimeJump(Constants.JumpDirection.Backward, true);

		if (Input.GetKeyDown(KeyCode.E)) TimeJump(Constants.JumpDirection.Forward, true);

		if (Input.GetKeyUp(KeyCode.Q)) TimeJump(Constants.JumpDirection.Backward, false);

		if (Input.GetKeyUp(KeyCode.E)) TimeJump(Constants.JumpDirection.Forward, false);

		if (Input.GetMouseButtonDown(0)) Grab();

		if (Input.GetKeyDown(KeyCode.Return)) StartGame();

		if (Input.GetKeyDown(KeyCode.Escape) && _preGame != null) _preGame.StopCountingDown();

		if (Input.GetKeyDown(KeyCode.L)) _timelord.SnapshotStates("GameSnapshot.txt");
	}


	private void UpdateTimeTravel()
	{
		// Record your state in all realities you exist in.
		Vector3 pos = Movement.GetPosition();
		Quaternion rot = Movement.GetRotation();
		Constants.JumpDirection dir = Constants.JumpDirection.Static;

		// Only set the jump direction once, when the _setJumpState flag is active.
		if (_setJumpState)
		{
			dir = _jumpDirection;
			_setJumpState = false;
		}

		PlayerState ps = new PlayerState(View.ViewID, pos, rot, dir, _isJumping);
		_timelord.RecordState(ps);

		if (_isJumping)
		{
			// Perform the time jump.
			if (_timelord.CanJump(View.ViewID, _jumpDirection))
			{
				_timelord.TimeTravel(View.ViewID, _jumpDirection);
			}
			// Force stop jumping.
			else
			{
				TimeJump(_jumpDirection, false);
			}
		}
		else _jumpDirection = Constants.JumpDirection.Static;
	}

	private void UpdateNameTag()
	{
		if(gameObject.layer == Constants.LayerOutsideReality)
		{
			Nametag.SetActive(false);
		}
		else Nametag.SetActive(true);
	} 


	void Update() {
		// Local keys only affect client's player.
		if (View.IsMine)
		{
			if (SceneManager.GetActiveScene().name == "PreGameScene" ||
			(SceneManager.GetActiveScene().name == "GameScene" && !_game.GameEnded))
			{
				UpdateCooldowns();
				KeyControl();
			}
			else if (SceneManager.GetActiveScene().name == "GameScene" && _game.GameEnded)
			{
				Movement.SetActive(false);
			}

			// Update pauseUI and cursor lock if game is ended.
			if (SceneManager.GetActiveScene().name == "GameScene" && _game.GameEnded)
			{
				PauseUI.Pause();
			}
		}
		// Comment the following line to show player name tags for testing interaction.
		else UpdateNameTag();
		if (TimeTravelEnabled()) UpdateTimeTravel();		
	}


	// ------------ IMPLEMENTED INTERFACE METHODS ------------

	public void NotifyStoppedDissolving(bool dissolvedOut)
	{
		if (!View.IsMine && dissolvedOut)
		{
			gameObject.layer = Constants.LayerOutsideReality;
		}
	}

	public Hashtable GetDebugValues()
	{
		Hashtable debugItems = new Hashtable();
		debugItems.Add("Room", PhotonNetwork.CurrentRoom.Name);
		debugItems.Add("Sprint", Input.GetKey("left shift"));
		debugItems.Add("Grab", _damageWindow);		
		return debugItems;
	}

	// ------------ PUBLIC METHODS ------------

	public void NotifyStartedDissolving()
	{
		gameObject.layer = Constants.LayerPlayer;
	}

	public void SetTimeLord(TimeLord timelord)
	{
		_timelord = timelord;
		_timelord.Connect(View.ViewID, View.IsMine);
		_timelord.EnterReality(View.ViewID);
		if (View.IsMine)
		{
			_tailManager.SetTimeLord(_timelord);
			Hud.SetTimeLord(timelord);
			_debugPanel.Register(_timelord);
		}
	}

	public void SetGame(GameController game)
	{
		_game = game;
		Movement.SetGame(game);
		Hud.SetGame(game);
		_collectingCrystals.SetGame(game);
		gameObject.layer = Constants.LayerPlayer;
	}

	public int GetID() { return View.ViewID; }

	public void Show() { gameObject.layer = Constants.LayerPlayer; }

	public void Hide() { gameObject.layer = Constants.LayerOutsideReality; }

	public (float forward, float back) GetCooldowns() { return (_forwardsJumpCooldown, _backJumpCooldown); }

	public (bool forward, bool back) GetCanJump()
	{
		bool canJumpForward = TimeTravelEnabled() && CanTimeTravel(Constants.JumpDirection.Forward);
		bool canJumpBack = TimeTravelEnabled() && CanTimeTravel(Constants.JumpDirection.Backward);
		return (canJumpForward, canJumpBack);
	}
}
