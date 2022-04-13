using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CrystalManager : MonoBehaviour
{

  //references to gameobjects
  [SerializeField] private GameObject spawnHolder;
  [SerializeField] private GameObject crystalPrefab;
  [SerializeField] private GameController game;

  //spawnPoints will have a location of every single crystal
  //order is shared with crystals list so can be iterated over simultaneously
  //spawnpoints necessary for crystal instantiation - add more by adding spawn points in editor
  private Transform[] spawnPoints;
  public List<CrystalBehaviour> crystals = new List<CrystalBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
      //find all spawnpoints in gamescene
      spawnPoints = new Transform[spawnHolder.transform.childCount];
      int i = 0;
      foreach(Transform child in spawnHolder.transform){
        spawnPoints[i++] = child;
      }

      //instantiate crystals (they will populate the crystals list themselves)
      foreach(Transform spawnPoint in spawnPoints){
        PhotonNetwork.Instantiate(crystalPrefab.name, spawnPoint.position, Quaternion.identity);
      }
    }

    // Update is called once per frame
    void Update()
    {
      //check if player should be able to see crystals
      //TODO::> set crystal layers???
      foreach(CrystalBehaviour crystal in crystals){
        float percievedTime = (game._timeLord.GetYourPosition()*game._timeLord._totalFrames) / Constants.FrameRate;
        if(percievedTime >= crystal.existanceRange[0] && percievedTime <= crystal.existanceRange[1]){
          crystal.gameObject.SetActive(true);//set layers here
        } else {
          crystal.gameObject.SetActive(false);//set layers here
        }
      }
    }

    //enumerator coroutine to be called when crystal is collected - waits x seconds before respawning crystal at random time
    public IEnumerator Respawn(int id){
      float spawnDelay = 5.0f;
      float newExistance = Random.Range(0.0f, (game._timeLord.GetTimeProportion()*game._timeLord._totalFrames)/Constants.FrameRate);
      Vector2 newExistanceRange = new Vector2(newExistance, newExistance + 10f );

      yield return new WaitForSeconds(spawnDelay);

      crystals[id].gameObject.GetComponent<PhotonView>().RPC("RPC_setExistanceRange", RpcTarget.All, newExistanceRange);
    }
}
