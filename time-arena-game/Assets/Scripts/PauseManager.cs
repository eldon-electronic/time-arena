using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class PauseManager : MonoBehaviourPunCallbacks {

	//pause tracking vars
    private bool _paused = false;
	public GameObject PauseMenuUI;
	public PhotonView View;

	// Update is called once per frame
	void Update() {
		if (!View.IsMine) return;

		if (Input.GetKeyDown(KeyCode.Escape)) _paused = !_paused;
		
		PauseMenuUI.SetActive(_paused);
		Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
	}

	public void Pause() { _paused = true; }

	public void Resume() { _paused = false; }

	public bool IsPaused() { return _paused; }

	public void Leave() { Application.Quit(); }

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
}
