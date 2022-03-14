using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class PauseManager : MonoBehaviourPunCallbacks {

	//pause tracking vars
    public bool IsPaused = false;
	public GameObject PauseMenuUI;

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			IsPaused = !IsPaused;
			PauseMenuUI.SetActive(IsPaused);
			Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
		}
	}

	public void Resume() {
		IsPaused = false;
		PauseMenuUI.SetActive(IsPaused);
		Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
	}

	/* Work on this in the future. Pressing "Leave" should take the user back to main screen.
	private void disconnectPlayer() {
		StartCoroutine(DisconnectAndLoad());
	}

	IEnumerator DisconnectAndLoad() {
		PhotonNetwork.LeaveRoom();
		while (PhotonNetwork.InRoom) { // Busy waiting
			Debug.Log("Busy waiting");
			yield return null; 
		}
		SceneManager.LoadScene("MenuScene");
		Destroy(gameObject);
	}*/

	public void Leave() {
		Application.Quit();
	}
}
