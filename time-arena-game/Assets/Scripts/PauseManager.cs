using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PauseManager : MonoBehaviour {

	//pause tracking vars
    public bool IsPaused = false;
	public GameObject PauseMenuUI;

	// Update is called once per frame
	void Update() {
		if(Input.GetKeyDown(KeyCode.Escape)){
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

	private void disconnectPlayer() {
		StartCoroutine(DisconnectAndLoad());
	}

	IEnumerator DisconnectAndLoad() {
		PhotonNetwork.LeaveRoom();
		while (PhotonNetwork.InRoom) yield return null; // Busy waiting
		SceneManager.LoadScene("MenuScene");
		Destroy(gameObject);
	}

	public void Leave() {
		disconnectPlayer();
	}
}
