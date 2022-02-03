using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PauseManager : MonoBehaviour {

	//pause tracking vars
	public bool isPaused = false;
	public GameObject pauseMenuUI;

	// Update is called once per frame
	void Update() {
		//on user pressing esc
		if(Input.GetKeyDown(KeyCode.Escape)){
			//flip pause and show/remove menu and update cursor state
			isPaused = !isPaused;
			pauseMenuUI.SetActive(isPaused);
			Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
		}
	}

	public void Resume(){
		isPaused = !isPaused;
		pauseMenuUI.SetActive(isPaused);
		Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
	}

	public void ReturnToLobby(){
		PhotonNetwork.LeaveRoom();
		SceneManager.LoadScene("LobbyScene");
	}

	public void Quit(){
		PhotonNetwork.LeaveRoom();
		SceneManager.LoadScene("MenuScene");
	}
}
