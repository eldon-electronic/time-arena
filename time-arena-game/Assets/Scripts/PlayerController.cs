using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour {

	// variables defining player values
	public Camera cam;
	public GameObject cameraHolder;
	public Canvas UI;
	public int team; // 0 seeker 1 hider 
	private float ab1Cooldown = 0f;
	private float ab2Cooldown = 0f;
	private int timeJumpAmount = 100;

	// variables corresponding to UI
	public PauseManager pauseUI;
	public GameObject nametag;

    // variables corresponding to player Animations
	public Animator playerAnim;
	public Transform grabCheck;
	public LayerMask grabMask;
	private float grabCheckRadius = 1f;
	private bool damageWindow = false;

    // the photonView component that syncs with the network
	public PhotonView View;

	// Time control variables
	public TimeConn TimeTravel;

    // variables corresponding to the gamestate
    public GameController Game;
	public ParticleController Particles;
	public PlayerHud Hud;
	public PlayerMaterial Material;
	public PlayerMovement Movement;

	// Start is called before the first frame update
	void Start() {
		DontDestroyOnLoad(this.gameObject);
		team = (int) GameController.Teams.Seeker; // Initialise to Seeker but call chang
		changeTeam(); // set the player's colour depending on their team
		if (!View.IsMine) {
			// destroy other player cameras and ui in local environment
			Destroy(cam.gameObject);
			Destroy(UI.gameObject);
			gameObject.layer = 7;
		} else {
			// destroy your own nametag
			Destroy(nametag);
			gameObject.tag = "Client";
		}
        PhotonNetwork.AutomaticallySyncScene = true;      // allow master client to move players from one scene to another
        Cursor.lockState = CursorLockMode.Locked;         // lock players cursor to center screen
        SceneManager.activeSceneChanged += onSceneChange; // link scenechange event to onscenechange
	}

	// onSceneChange is called by the SceneManager.activeSceneChanged event;
	void onSceneChange(Scene current, Scene next) {
		if (next.name == "GameScene") {
			Game = FindObjectOfType<GameController>();
			TimeTravel.connectToTimeLord();
			if (Game == null) {
				Debug.Log("Scene change error: GameController is null");
			}
			ab1Cooldown = 15;
			ab2Cooldown = 15;
		}
	}

	// Update is called once per frame
	void Update() {
		// local keys only affect client's player
		if (!View.IsMine) return;

		if (SceneManager.GetActiveScene().name == "PreGameScene" ||
		(SceneManager.GetActiveScene().name == "GameScene" && !Game.gameEnded)) {
			UpdateCooldowns();
			KeyControl();
		}

		// Set the debug items and send to HUD to be displayed
		Hashtable debugItems = new Hashtable();
		debugItems.Add("Room", PhotonNetwork.CurrentRoom.Name);
		debugItems.Add("Sprint", Input.GetKey("left shift"));
		debugItems.Add("Grab", damageWindow);
		Hud.SetDebugValues(debugItems);

		// update player cooldown displays
		float[] cooldownValues = new float[]{1.0f - (ab1Cooldown / 15.0f), 1.0f - (ab2Cooldown / 15.0f)};
		Hud.SetCooldownValues(cooldownValues);

		bool canJumpForward = SceneManager.GetActiveScene().name == "GameScene" && ab1Cooldown <= 0.0f && 
							TimeTravel.GetRealityTick() + (float) timeJumpAmount <= TimeTravel.GetCurrentTick();
		bool canJumpBack = SceneManager.GetActiveScene().name == "GameScene" && ab2Cooldown <= 0.0f && 
							TimeTravel.GetRealityTick() - (float) timeJumpAmount >= 0;
		Hud.SetCanJump(canJumpForward, canJumpBack);

		// update pauseUI and cursor lock if game is ended
		if (SceneManager.GetActiveScene().name == "GameScene" && Game.gameEnded)
		{
			pauseUI.isPaused = true;
			pauseUI.pauseMenuUI.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
		}
	}

	/*****************
	* Button Presses *
	******************/

	void UpdateCooldowns()
	{
		ab1Cooldown = (ab1Cooldown > 0) ? (ab1Cooldown - Time.deltaTime) : 0;
		ab2Cooldown = (ab2Cooldown > 0) ? (ab2Cooldown - Time.deltaTime) : 0;
	}

	void KeyControl()
	{
		// Keypress '1' -> time jump backward
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			// only allow time travel backwards if it doesn't go past the beginning.
			if (SceneManager.GetActiveScene().name == "GameScene" && ab2Cooldown <= 0 &&
				TimeTravel.GetRealityTick() - (float) timeJumpAmount >= 0)
			{ 
				jumpBackwards();
			}
		}

		// Keypress '2' -> time jump forward
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			// only allow time travel forwards if it doesn't go past the end.
			if (SceneManager.GetActiveScene().name == "GameScene" && ab1Cooldown <= 0 &&
				TimeTravel.GetRealityTick() + (float) timeJumpAmount <= TimeTravel.GetCurrentTick())
			{
				jumpForward(); 
			}
		}

		// Left mouse click -> start grabbing
		if (Input.GetMouseButtonDown(0))
		{
			// if grabbing, check for intersection with player
			if (!damageWindow)
			{
				Collider[] playersGrab = Physics.OverlapSphere(grabCheck.position, grabCheckRadius, grabMask);
				foreach (var playerGotGrab in playersGrab)
				{
					// call grabplayer function on that player
					PlayerController targetPlayer = playerGotGrab.GetComponent<PlayerController>();
					if (team == (int) GameController.Teams.Seeker && 
						targetPlayer.team == (int) GameController.Teams.Hider)
					{
						targetPlayer.getFound();
					}
				}
				playerAnim.SetBool("isGrabbing", true);
			}
		}

		// Keypress 'e' -> start game
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient)
			{
				Hud.StartCountingDown();
			}
		}

		// Keypress `ESC` -> stop counting down to game launch
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Hud.StopCountingDown();
		}

		// Keypress 'p' -> toggle debug mode
		if (Input.GetKeyDown(KeyCode.P))
		{
			Hud.ToggleDebug();
		}
	}

	// change player teams
	public void changeTeam() {
		if (team == (int) GameController.Teams.Hider) {
			team = (int) GameController.Teams.Seeker;
			Material.SetMaterial("seeker");
			Hud.SetTeam("SEEKER");
		} else {
			team = (int) GameController.Teams.Hider;
			Material.SetMaterial("hider");
			Hud.SetTeam("HIDER");
		}
	}

	/************
	* RPC Calls *
	*************/

	[PunRPC]
	void RPC_jumpBackwards() {
		TimeTravel.TimeJump(-timeJumpAmount);
		Particles.StartJumpingBackward();
		ab2Cooldown = 15;
		Hud.PressForwardJumpButton();
		Game.otherPlayersElapsedTime[View.ViewID] -= timeJumpAmount / TimeTravel.MaxTick();
	}

	public void jumpBackwards() {
		View.RPC("RPC_jumpBackwards", RpcTarget.All);
	}

	[PunRPC]
	void RPC_jumpForward() {
		TimeTravel.TimeJump(timeJumpAmount);
		Particles.StartJumpingForward();
		ab1Cooldown = 15;
		Hud.PressBackJumpButton();
		Game.otherPlayersElapsedTime[View.ViewID] += timeJumpAmount / TimeTravel.MaxTick();
	}

	public void jumpForward() {
		View.RPC("RPC_jumpForward", RpcTarget.All);
	}

	// RPC function to be called when another player finds this one
	[PunRPC]
	void RPC_getFound() {
		changeTeam();
	}

	// RPC function to be called when another player hits this one
	// function to get found by calling RPC on all machines
	public void getFound(){
		View.RPC("RPC_getFound", RpcTarget.All);
	}

	// RPC function to be called by other machines to set this players transform
	[PunRPC]
	void RPC_movePlayer(Vector3 pos, Vector3 rot) {
		transform.position = pos;
		transform.rotation = Quaternion.Euler(rot);
		cameraHolder.transform.rotation = Quaternion.Euler(rot);
	}

	// function to move this player by calling RPC for all others
	public void movePlayer(Vector3 pos, Vector3 rot) {
		View.RPC("RPC_movePlayer", RpcTarget.All, pos, rot);
	}

	// function to enable player to damage others
	public void startGrabbing() {
		damageWindow = true;
	}

	// function to disable player to damage others
	public void stopGrabbing() {
		damageWindow = false;
		playerAnim.SetBool("isGrabbing", false);
	}

	// function called on game gameEnded
	public void onGameEnded(){
		/*if(PhotonNetwork.IsMasterClient){
			PhotonNetwork.LoadLevel("PreGameScene");
		}*/
	}
}
