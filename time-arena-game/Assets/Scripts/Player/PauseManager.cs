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

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_paused = !_paused;
			_pauseMenuUI.SetActive(_paused);
			Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
		}
	}

	private void OnGameEnded(Constants.Team team) { _paused = true; }

	public void OnResume() { _paused = false; }
	
	// Work on this in the future. Pressing "Leave" should take the user back to main screen.
	public void OnLeave()
	{
		PhotonNetwork.LeaveRoom();
		SceneManager.LoadScene("MenuScene");
		Destroy(gameObject);
	}

  /*	StartCoroutine(DisconnectAndLoad());
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

	public bool IsPaused() { return _paused; }
}
