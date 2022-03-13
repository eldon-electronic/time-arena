using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour {

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
	public TimeConn TimeTravel;

    // Variables corresponding to the gamestate.
    public GameController Game;

	// 0 seeker 1 hider.
	public int Team;
	private float _forwardsJumpCooldown = 0f;
	private float _backJumpCooldown = 0f;
	private int _timeJumpAmount = 100;
	private Vector3[] _hiderSpawnPoints;
	private Vector3 _seekerSpawnPoint;


	void Start() {
		DontDestroyOnLoad(this.gameObject);
		Team = (int) GameController.Teams.Hider;
		Material.SetHiderMaterial();
		Hud.SetTeam("HIDER");
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
	}


	private void MoveToSpawnPoint()
	{
		if (Team == (int) GameController.Teams.Hider)
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
			Game = FindObjectOfType<GameController>();
			if (Game == null)
			{
				Debug.Log("Scene change error: GameController is null");
			}

			TimeTravel.connectToTimeLord();

			_forwardsJumpCooldown = 15;
			_backJumpCooldown = 15;

			MoveToSpawnPoint();
			Material.SetArmActive(Team == (int) GameController.Teams.Seeker);
		}
	}


	// ------------ RPC FUNCTIONS ------------


	[PunRPC]
	void RPC_jumpBackwards()
	{
		TimeTravel.TimeJump(-_timeJumpAmount);
		Particles.StartJumpingBackward();
		_backJumpCooldown = 15;
		Hud.PressForwardJumpButton();
		Game.otherPlayersElapsedTime[View.ViewID] -= _timeJumpAmount / TimeTravel.MaxTick();
	}

	[PunRPC]
	void RPC_jumpForward()
	{
		TimeTravel.TimeJump(_timeJumpAmount);
		Particles.StartJumpingForward();
		_forwardsJumpCooldown = 15;
		Hud.PressBackJumpButton();
		Game.otherPlayersElapsedTime[View.ViewID] += _timeJumpAmount / TimeTravel.MaxTick();
	}

	// RPC function to be called when another player finds this one.
	[PunRPC]
	void RPC_getFound()
	{
		ChangeTeam();
	}

	// RPC function to be called by other machines to set this players transform.
	[PunRPC]
	void RPC_movePlayer(Vector3 pos, Vector3 rot)
	{
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
		CameraHolder.transform.rotation = Quaternion.Euler(rot);
	}


	// ------------ ACTIONS ------------


	public void JumpBackwards()
	{
		// Only allow time travel backwards if it doesn't go past the beginning.
		if (SceneManager.GetActiveScene().name == "GameScene" && _backJumpCooldown <= 0 &&
			TimeTravel.GetRealityTick() - (float) _timeJumpAmount >= 0 && !Particles.IsJumping())
		{
			View.RPC("RPC_jumpBackwards", RpcTarget.All);
		}
	}

	public void JumpForward()
	{
		// Only allow time travel forwards if it doesn't go past the end.
		if (SceneManager.GetActiveScene().name == "GameScene" && _forwardsJumpCooldown <= 0 &&
			TimeTravel.GetRealityTick() + (float) _timeJumpAmount <= TimeTravel.GetCurrentTick() &&
			!Particles.IsJumping())
		{
			View.RPC("RPC_jumpForward", RpcTarget.All);
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
				if (Team == (int) GameController.Teams.Seeker && 
					targetPlayer.Team == (int) GameController.Teams.Hider)
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
		if (Team == (int) GameController.Teams.Hider)
		{
			Team = (int) GameController.Teams.Seeker;
			Material.SetMaterial("seeker");
			Material.SetArmActive(true);
			Hud.SetTeam("SEEKER");
		}
		else
		{
			Team = (int) GameController.Teams.Hider;
			Material.SetMaterial("hider");
			Material.SetArmActive(false);
			Hud.SetTeam("HIDER");
		}
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

	// Function called on game gameEnded.
	public void OnGameEnded()
	{
		/*if(PhotonNetwork.IsMasterClient){
			PhotonNetwork.LoadLevel("PreGameScene");
		}*/
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

		bool canJumpForward = SceneManager.GetActiveScene().name == "GameScene" && _forwardsJumpCooldown <= 0.0f && 
							TimeTravel.GetRealityTick() + (float) _timeJumpAmount <= TimeTravel.GetCurrentTick();
		bool canJumpBack = SceneManager.GetActiveScene().name == "GameScene" && _backJumpCooldown <= 0.0f && 
							TimeTravel.GetRealityTick() - (float) _timeJumpAmount >= 0;
		Hud.SetCanJump(canJumpForward, canJumpBack);
	}

	private void UpdateDebugDisplay()
	{
		Hashtable debugItems = new Hashtable();
		debugItems.Add("Room", PhotonNetwork.CurrentRoom.Name);
		debugItems.Add("Sprint", Input.GetKey("left shift"));
		debugItems.Add("Grab", _damageWindow);

		Hashtable movementState = Movement.GetState();
		Utilities.Union(ref debugItems, movementState);
		
		Hud.SetDebugValues(debugItems);
	}

	void KeyControl()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1)) JumpBackwards();

		if (Input.GetKeyDown(KeyCode.Alpha2)) JumpForward();

		if (Input.GetMouseButtonDown(0)) Grab();

		if (Input.GetKeyDown(KeyCode.E)) StartGame();

		if (Input.GetKeyDown(KeyCode.Escape)) Hud.StopCountingDown();

		if (Input.GetKeyDown(KeyCode.P)) Hud.ToggleDebug();
	}

	void Update() {
		// Local keys only affect client's player.
		if (!View.IsMine) return;

		if (SceneManager.GetActiveScene().name == "PreGameScene" ||
		(SceneManager.GetActiveScene().name == "GameScene" && !Game.gameEnded)) {
			UpdateCooldowns();
			UpdateDebugDisplay();
			KeyControl();
		}

		// Update pauseUI and cursor lock if game is ended.
		if (SceneManager.GetActiveScene().name == "GameScene" && Game.gameEnded)
		{
			PauseUI.isPaused = true;
			PauseUI.pauseMenuUI.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
		}
	}
}
