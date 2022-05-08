using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CrystalManager : MonoBehaviour
{

  //references to gameobjects
  [SerializeField] private GameObject _spawnHolder;
  [SerializeField] private GameObject _crystalPrefab;
  [SerializeField] private GameController _game;

  //spawnPoints will have a location of every single crystal
  //order is shared with crystals list so can be iterated over simultaneously
  //spawnpoints necessary for crystal instantiation - add more by adding spawn points in editor
  private Transform[] _spawnPoints;
  public List<CrystalBehaviour> Crystals = new List<CrystalBehaviour>();
  private bool _spawned = false;

    // Start is called before the first frame update
    void Start()
    {
      //find all spawnpoints in gamescene
      _spawnPoints = new Transform[_spawnHolder.transform.childCount];
      int i = 0;
      foreach(Transform child in _spawnHolder.transform){
        _spawnPoints[i++] = child;
      }

      if(PhotonNetwork.IsMasterClient){
        //instantiate crystals (they will populate the crystals list themselves)
        foreach(Transform spawnPoint in _spawnPoints){
          PhotonNetwork.Instantiate(_crystalPrefab.name, spawnPoint.position, Quaternion.identity);
        }
      }
    }

    // Update is called once per frame
    void Update()
    {
      if(PhotonNetwork.IsMasterClient && !_spawned){
        foreach(CrystalBehaviour crystal in Crystals){
          StartCoroutine(Respawn(crystal.ID));
        }
      }
      _spawned = true;
      //check if player should be able to see crystals
      foreach(CrystalBehaviour crystal in Crystals){
        TimeLord t = _game.GetTimeLord();
        float percievedTime = (float)(t.GetYourPerceivedFrame()) / Constants.FrameRate;
        if(percievedTime >= crystal.ExistanceRange[0] && percievedTime <= crystal.ExistanceRange[1]){
          if(!crystal.gameObject.activeSelf){
            crystal.UpdateAnim();
            crystal.T = 0;
            crystal.gameObject.SetActive(true);
            crystal.gameObject.layer = 3;
          }
        } else {
          crystal.gameObject.SetActive(false);
          crystal.gameObject.layer = 9;
        }
      }

    }

    //enumerator coroutine to be called when crystal is collected - waits x seconds before respawning crystal at random time
    public IEnumerator Respawn(int id){
      float spawnDelay = 5.0f;
      TimeLord t = _game.GetTimeLord();
      float newExistance = Random.Range( (t.GetCurrentFrame()/Constants.FrameRate) -5f, Mathf.Max(t.GetCurrentFrame()/Constants.FrameRate, 10)+5f);
      float existanceLength = Crystals[id].gameObject.GetComponent<CrystalBehaviour>().ExistanceLength;
      Vector2 newExistanceRange = new Vector2(newExistance, newExistance + existanceLength );

      yield return new WaitForSeconds(spawnDelay);

      Crystals[id].gameObject.GetComponent<PhotonView>().RPC("RPC_setExistanceRange", RpcTarget.All, newExistanceRange);
    }
}
