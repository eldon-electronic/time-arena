using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PauseManager : MonoBehaviourPunCallbacks
{
	private bool _paused = false;
	public GameObject PauseMenuUI;
	public PhotonView View;
	public Slider MouseSensSlider;
  	public float MouseSens;

	void Update()
	{	
		if (!View.IsMine) return;

		// TODO: remove this
		if (MouseSensSlider == null) Debug.LogError("MouseSensSlider is null");
		if (PauseMenuUI == null) Debug.LogError("PauseMenuUI is null");

		MouseSens = MouseSensSlider.value;
		if (Input.GetKeyDown(KeyCode.Escape)) _paused = !_paused;

		PauseMenuUI.SetActive(_paused);
		Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
	}

	public void Pause() { _paused = true; }

	public void Resume() { _paused = false; }

	public bool IsPaused() { return _paused; }

	public void Leave() { disconnectPlayer(); }

	// Work on this in the future. Pressing "Leave" should take the user back to main screen.
	private void disconnectPlayer() {
    PhotonNetwork.LeaveRoom();
    SceneManager.LoadScene("MenuScene");
    Destroy(gameObject);
  }
}
