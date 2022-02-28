using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    // public Canvas UI;
	// public PauseManager pauseUI;
	// public Text debugMenu_speed;
	// public Text debugMenu_room;
	// public Text debugMenu_sprint;
	// public Text debugMenu_grab;
	// public Text debugMenu_ground;
	// public Text masterClientOpts;
	// public Text ab1Cooldown_displ;
	// public Text ab2Cooldown_displ;
	// public Text ab3Cooldown_displ;
	// public Text teamDispl;
	// public Text timeDispl;
	// public Text startTimeDispl;
	// public Text winningDispl;
	// private float secondsTillGame;
	// private bool isCountingTillGameStart;
    // public Slider elapsedTimeSlider;
    // public Slider playerIcon;
	// public Slider otherPlayerIcon1;
    // public Slider otherPlayerIcon2;
    // public Slider otherPlayerIcon3;
    // public Slider otherPlayerIcon4;
	// private Slider[] playerIcons = new Slider[5];
	// public Image Forward;
	// public Image ForwardPressed;
	// public Image ForwardUnable;
    // public PhotonView view;
    
    // void Start()
    // {
    //     playerIcons[0] = playerIcon;
	// 	playerIcons[1] = otherPlayerIcon1;
	// 	playerIcons[2] = otherPlayerIcon2;
	// 	playerIcons[3] = otherPlayerIcon3;
	// 	playerIcons[4] = otherPlayerIcon4;

    //     view = GetComponent<PhotonView>();
    //     if (!view.IsMine)
    //     {
    //         Destroy(UI);
    //     }
    // }

    
    // void LateUpdate()
    // {
    //     if (view.IsMine)
    //     {
    //         // if master client, show 'press e o start' text or 'starting in' text
    //         masterClientOpts.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient);
    //         teamDispl.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
    //         timeDispl.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
    //         startTimeDispl.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
    //         elapsedTimeSlider.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
    //         playerIcon.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
	// 		otherPlayerIcon1.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene" && game.otherPlayersElapsedTime.Count >= 2);
	// 		otherPlayerIcon2.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene" && game.otherPlayersElapsedTime.Count >= 3);
	// 		otherPlayerIcon3.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene" && game.otherPlayersElapsedTime.Count >= 4);
	// 		otherPlayerIcon4.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene" && game.otherPlayersElapsedTime.Count >= 5);
    //     }

    //     if(isCountingTillGameStart)
    //     {
	// 		masterClientOpts.text = "Starting in " + System.Math.Round (secondsTillGame, 0) + "s";
	// 		if(System.Math.Round (secondsTillGame, 0) <= 0.0f)
    //         {
	// 			// PhotonNetwork.Room.open = false;
	// 			masterClientOpts.text = "Loading...";
	// 		}
	// 	}

    //     Vector3 movementVector = transform.position - lastPos;
    //     float distTravelled = movementVector.magnitude / Time.deltaTime;
    //     debugMenu_speed.text = "Speed: " + distTravelled;
    //     debugMenu_room.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
    //     debugMenu_sprint.text = "Sprint: " + Input.GetKey("left shift");
    //     debugMenu_grab.text = "Grab: " + damageWindow;
    //     debugMenu_ground.text = "Ground: " + isGrounded;

    //     // update player ability displays
    //     ab1Cooldown_displ.text = "" + (int)ab1Cooldown;
    //     ab2Cooldown_displ.text = "" + (int)ab2Cooldown;
    //     ab3Cooldown_displ.text = "" + (int)ab3Cooldown;

    //     // update winningTeam Text

    //     // update gametimer
    //     if (SceneManager.GetActiveScene().name == "GameScene")
    //     {
    //         float t = game.gameLength - game.timeElapsedInGame;
    //         startTimeDispl.transform.parent.gameObject.SetActive(!game.gameStarted);
    //         if (game.gameStarted && !game.gameEnded)
    //         {
    //             timeDispl.text = (int)(t/60) + ":" + ((int)(t%60)).ToString().PadLeft(2, '0') + ":" + (((int)(((t%60)-(int)(t%60))*100))*60/100).ToString().PadLeft(2, '0');
    //             elapsedTimeSlider.value = game.timeElapsedInGame / game.gameLength; // update time bar
    //             int n = 0;
    //             List<int> keys = new List<int>(game.otherPlayersElapsedTime.Keys);
    //             foreach(int key in keys)
    //             {
    //                 playerIcons[n].value = game.otherPlayersElapsedTime[key];
    //                 n++;
    //             }
    //         }
    //         else if(game.gameEnded)
    //         {
    //             winningDispl.transform.parent.gameObject.SetActive(true);
    //             winningDispl.text = (game.winningTeam == 1) ? "HIDERS WIN!" : "SEEKERS WIN!";
    //             pauseUI.isPaused = true;
    //             pauseUI.pauseMenuUI.SetActive(true);
    //             Cursor.lockState = CursorLockMode.None;
    //         }
    //         else
    //         {
    //             startTimeDispl.text = "" + (5-(int)(game.timeElapsedInGame+0.9f));
    //             timeDispl.text = "0:00:00";
    //             playerIcon.value = 0;
    //             int n = 0;
    //             List<int> keys = new List<int>(game.otherPlayersElapsedTime.Keys);
    //             foreach(int key in keys)
    //             {
    //                 playerIcons[n].value = 0;
    //                 n++;
    //             }
    //         }
    //     }
    // }

    // void keyControl()
    // {
    //     if (SceneManager.GetActiveScene().name == "PreGameScene" ||
    //     (SceneManager.GetActiveScene().name == "GameScene" && game.gameStarted))
    //     {
    //         // start game onpress 'e'
	// 		if (SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient &&
	// 		Input.GetKeyDown(KeyCode.E) && !isCountingTillGameStart)
    //         {
	// 			isCountingTillGameStart = true;
	// 			secondsTillGame = 5.0f;
	// 		}
	// 		// if counting for game launch and user presses esc - stop
	// 		if (Input.GetKeyDown(KeyCode.Escape))
    //         {
	// 			isCountingTillGameStart = false;
	// 			secondsTillGame = 0;
	// 		}
	// 		// if counting, reduce timer
	// 		if (PhotonNetwork.IsMasterClient && isCountingTillGameStart)
    //         {
	// 			secondsTillGame -= Time.deltaTime;
	// 			if (secondsTillGame <= 0)
    //             {
	// 				PhotonNetwork.LoadLevel("GameScene");
	// 				isCountingTillGameStart = false;
	// 			}
	// 		}
    //     }
    // }

    // public void changeTeam()
    // {
    //     if (team == 0)
    //     {
    //         teamDispl.text = "SEEKER";
    //     }
    //     else
    //     {
    //         teamDispl.text = "HIDER";
    //     }
    // }
}
