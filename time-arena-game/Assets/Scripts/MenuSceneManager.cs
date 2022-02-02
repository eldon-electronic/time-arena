using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneManager : MonoBehaviour{

	//user presses start button
	public void StartGame(){
		SceneManager.LoadScene("ConnectingScene");
	}

	//user presses quit button
	public void QuitGame(){
		Application.Quit();
	}
}
