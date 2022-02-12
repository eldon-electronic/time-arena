using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks{

	//values for input text fields set by user
	public InputField createInput;
	public InputField joinInput;

	//panels containing the browser vs creator view for servers
	public GameObject serverNameJoin;
	public GameObject serverBrowser;

	//container into which servers created from prefab will be placed
	public GameObject serverContainer;
	public GameObject serverPrefab;

	//dictioary containing server names and roomInfo for each room
	public Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

	//use cachedroomlist to populate scrollbox
	public void populateServerList(){
		while (serverContainer.transform.childCount > 0) {
			DestroyImmediate(serverContainer.transform.GetChild(0).gameObject);
		}
		foreach(KeyValuePair<string, RoomInfo> room in cachedRoomList){
			Debug.Log("hello");
			GameObject newServerButton = Instantiate(serverPrefab) as GameObject;
			newServerButton.transform.parent = serverContainer.transform;
			newServerButton.GetComponent<ServerButton>().serverName = room.Key;
		}
	}

	//update local;y cached roomlist with roomlist recieved from onroomlistupdate
	public void UpdateCachedRoomList(List<RoomInfo> roomList){
		for(int i=0; i<roomList.Count; i++){
			RoomInfo info = roomList[i];
			if (info.RemovedFromList){
				cachedRoomList.Remove(info.Name);
			} else {
				cachedRoomList[info.Name] = info;
			}
		}
	}

	//onroomlistupdate call method to cache into local memory
	public override void OnRoomListUpdate(List<RoomInfo> roomList){
		UpdateCachedRoomList(roomList);
	}

	//user presses create room button
	public void CreateRoom(){
		PhotonNetwork.CreateRoom(createInput.text);
	}

	//user presses join room button
	public void JoinRoom(){
		PhotonNetwork.JoinRoom(joinInput.text);
	}

	//when user connects to room - load scene as level
	public override void OnJoinedRoom(){
		PhotonNetwork.LoadLevel("PreGameScene");
	}

	//onpress of back button - return to home screen and disconnect
	public void Back(){
		SceneManager.LoadScene("MenuScene");
		PhotonNetwork.Disconnect();
	}

	//onpress of serverView button toggle server browser visibillity
	public void toggleServerView(){
		serverNameJoin.SetActive(!serverNameJoin.activeSelf);
		serverBrowser.SetActive(!serverBrowser.activeSelf);
	}

	//clear serverlist upon leaving lobby
	public override void OnLeftLobby(){
		cachedRoomList.Clear();
	}

	//clear serverlist upon disconnect
	public override void OnDisconnected(DisconnectCause cause){
		cachedRoomList.Clear();
	}
}
