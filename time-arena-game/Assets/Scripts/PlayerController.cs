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
	public Canvas UI;
	// 0 seeker 1 hider.
	public int Team;

	// Variables corresponding to UI.
	public PauseManager PauseUI;
	public GameObject Nametag;

    // Variables corresponding to player Animations.
	public Animator PlayerAnim;
	public Transform GrabCheck;
	public LayerMask GrabMask;
	private float _grabCheckRadius = 1f;
	private bool _damageWindow = false;

    // The photonView component that syncs with the network.
	public PhotonView View;

	// Time control variables.
	public TimeConn TimeTravel;

    // Variables corresponding to the gamestate.
    public GameController Game;
	public ParticleController Particles;
	public PlayerHud Hud;
	public PlayerMaterial Material;
	public PlayerMovement Movement;

	private float _forwardsJumpCooldown = 0f;
	private float _backJumpCooldown = 0f;
	private int _timeJumpAmount = 100;

	// Start is called before the first frame update.
	void Start() {
		DontDestroyOnLoad(this.gameObject);
		Team = (int) GameController.Teams.Seeker;
		ChangeTeam();
		if (!View.IsMine) {
			Destroy(Cam.gameObject);
			Destroy(UI.gameObject);
			gameObject.layer = 7;
		} else {
			Destroy(Nametag);
			gameObject.tag = "Client";
		}
		// Allow master client to move players from one scene to another.
        PhotonNetwork.AutomaticallySyncScene = true;
		// Lock players cursor to center screen.
        Cursor.lockState = CursorLockMode.Locked;
		// Link scenechange event to onscenechange.
        SceneManager.activeSceneChanged += OnSceneChange;
	}

	// onSceneChange is called by the SceneManager.activeSceneChanged event.
	void OnSceneChange(Scene current, Scene next) {
		if (next.name == "GameScene") {
			Game = FindObjectOfType<GameController>();
			TimeTravel.connectToTimeLord();
			if (Game == null) {
				Debug.Log("Scene change error: GameController is null");
			}
			_forwardsJumpCooldown = 15;
			_backJumpCooldown = 15;
		}
	}

	void Update() {
		// Local keys only affect client's player.
		if (!View.IsMine) return;

		if (SceneManager.GetActiveScene().name == "PreGameScene" ||
		(SceneManager.GetActiveScene().name == "GameScene" && !Game.gameEnded)) {
			UpdateCooldowns();
			KeyControl();
		}

		// Set the debug items and send to HUD to be displayed.
		Hashtable debugItems = new Hashtable();
		debugItems.Add("Room", PhotonNetwork.CurrentRoom.Name);
		debugItems.Add("Sprint", Input.GetKey("left shift"));
		debugItems.Add("Grab", _damageWindow);
		Hud.SetDebugValues(debugItems);

		// Update player cooldown displays.
		float[] cooldownValues = new float[]{1.0f - (_forwardsJumpCooldown / 15.0f), 1.0f - (_backJumpCooldown / 15.0f)};
		Hud.SetCooldownValues(cooldownValues);

		bool canJumpForward = SceneManager.GetActiveScene().name == "GameScene" && _forwardsJumpCooldown <= 0.0f && 
							TimeTravel.GetRealityTick() + (float) _timeJumpAmount <= TimeTravel.GetCurrentTick();
		bool canJumpBack = SceneManager.GetActiveScene().name == "GameScene" && _backJumpCooldown <= 0.0f && 
							TimeTravel.GetRealityTick() - (float) _timeJumpAmount >= 0;
		Hud.SetCanJump(canJumpForward, canJumpBack);

		// Update pauseUI and cursor lock if game is ended.
		if (SceneManager.GetActiveScene().name == "GameScene" && Game.gameEnded)
		{
			PauseUI.isPaused = true;
			PauseUI.pauseMenuUI.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
		}
	}

	void UpdateCooldowns()
	{
		_forwardsJumpCooldown = (_forwardsJumpCooldown > 0) ? (_forwardsJumpCooldown - Time.deltaTime) : 0;
		_backJumpCooldown = (_backJumpCooldown > 0) ? (_backJumpCooldown - Time.deltaTime) : 0;
	}

	void KeyControl()
	{
		// Keypress '1' -> time jump backward.
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			// Only allow time travel backwards if it doesn't go past the beginning.
			if (SceneManager.GetActiveScene().name == "GameScene" && _backJumpCooldown <= 0 &&
				TimeTravel.GetRealityTick() - (float) _timeJumpAmount >= 0)
			{ 
				JumpBackwards();
			}
		}

		// Keypress '2' -> time jump forward.
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			// Only allow time travel forwards if it doesn't go past the end.
			if (SceneManager.GetActiveScene().name == "GameScene" && _forwardsJumpCooldown <= 0 &&
				TimeTravel.GetRealityTick() + (float) _timeJumpAmount <= TimeTravel.GetCurrentTick())
			{
				JumpForward(); 
			}
		}

		// Left mouse click -> start grabbing.
		if (Input.GetMouseButtonDown(0))
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

		// Keypress 'e' -> start game.
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient)
			{
				Hud.StartCountingDown();
			}
		}

		// Keypress `ESC` -> stop counting down to game launch.
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Hud.StopCountingDown();
		}

		// Keypress 'p' -> toggle debug mode.
		if (Input.GetKeyDown(KeyCode.P))
		{
			Hud.ToggleDebug();
		}
	}

	// Change player teams.
	public void ChangeTeam()
	{
		if (Team == (int) GameController.Teams.Hider)
		{
			Team = (int) GameController.Teams.Seeker;
			Material.SetMaterial("seeker");
			Hud.SetTeam("SEEKER");
		}
		else
		{
			Team = (int) GameController.Teams.Hider;
			Material.SetMaterial("hider");
			Hud.SetTeam("HIDER");
		}
	}

	[PunRPC]
	void RPC_jumpBackwards()
	{
		TimeTravel.TimeJump(-_timeJumpAmount);
		Particles.StartJumpingBackward();
		_backJumpCooldown = 15;
		Hud.PressForwardJumpButton();
		Game.otherPlayersElapsedTime[View.ViewID] -= _timeJumpAmount / TimeTravel.MaxTick();
	}

	public void JumpBackwards()
	{
		View.RPC("RPC_jumpBackwards", RpcTarget.All);
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

	public void JumpForward()
	{
		View.RPC("RPC_jumpForward", RpcTarget.All);
	}

	// RPC function to be called when another player finds this one.
	[PunRPC]
	void RPC_getFound()
	{
		ChangeTeam();
	}

	// RPC function to be called when another player hits this one.
	// Function to get found by calling RPC on all machines.
	public void GetFound()
	{
		View.RPC("RPC_getFound", RpcTarget.All);
	}

	// RPC function to be called by other machines to set this players transform.
	[PunRPC]
	void RPC_movePlayer(Vector3 pos, Vector3 rot)
	{
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
		CameraHolder.transform.rotation = Quaternion.Euler(rot);
	}

	// Function to move this player by calling RPC for all others.
	public void MovePlayer(Vector3 pos, Vector3 rot)
	{
		View.RPC("RPC_movePlayer", RpcTarget.All, pos, rot);
	}

	// Function to enable player to damage others.
	public void StartGrabbing()
	{
		_damageWindow = true;
	}

	// Function to disable player to damage others.
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
}
