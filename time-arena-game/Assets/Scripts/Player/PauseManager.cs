using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PauseManager : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject _pauseMenuUI;
	[SerializeField] private PhotonView _view;
	private bool _paused;
	public static event Action<bool> paused;

	void OnEnable()
	{
		GameController.gameEnded += OnGameEnded;
	}

	void OnDisable()
	{
		GameController.gameEnded -= OnGameEnded;
	}

	void Start()
	{
		_paused = false;
	}

	void Update()
	{	
		if (!_view.IsMine) return;
		if (Input.GetKeyDown(KeyCode.Escape)) SetPause(!_paused);
	}

	private void OnGameEnded(Constants.Team team) { SetPause(true); }

	public void OnResume() { SetPause(false); }

	// Work on this in the future. Pressing "Leave" should take the user back to main screen.
	public void OnLeave()
	{
		PhotonNetwork.LeaveRoom();
		SceneManager.LoadScene("MenuScene");
		Destroy(gameObject);
	}

	private void SetPause(bool pause)
	{
		_paused = pause;
		_pauseMenuUI.SetActive(_paused);
		Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
		paused?.Invoke(_paused);
	}
}
