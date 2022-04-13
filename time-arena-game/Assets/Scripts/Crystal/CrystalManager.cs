using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CrystalManager : MonoBehaviour
{

  [SerializeField] private GameObject crystal_prefab;
  [SerializeField] private GameController _game;

  private List<CrystalBehaviour> crystals = new List<CrystalBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
      if(PhotonNetwork.IsMasterClient){
        spawnCrystals();
      }
    }

    public void collected(int id){
      crystals[id].gameObject.SetActive(false);
      int i = 0;
      foreach(Vector2 existanceRange in crystals[id].existanceRanges){
        if(_game.GetCurrentTime() >= existanceRange[0] && _game.GetCurrentTime() <= existanceRange[1]){
          break;
        }
        i++;
      }
      crystals[id].existanceRanges.RemoveAt(i);
    }

    public void spawnCrystals(){
      spawnCollectableCrystal(new Vector3(Random.Range(-10f, 10f), -3, Random.Range(-10f, 10f)));
      spawnCollectableCrystal(new Vector3(Random.Range(-10f, 10f), -3, Random.Range(-10f, 10f)));
      spawnCollectableCrystal(new Vector3(Random.Range(-10f, 10f), -3, Random.Range(-10f, 10f)));
    }


    private void spawnCollectableCrystal(Vector3 spawnLoc){
      PhotonNetwork.Instantiate(crystal_prefab.name, spawnLoc, Quaternion.identity);
    }

    public int addCrystal(CrystalBehaviour newCrystal){
      crystals.Add(newCrystal);
      return crystals.Count-1;
    }


    // Update is called once per frame
    void Update()
    {
      foreach(CrystalBehaviour crystal in crystals){
        crystal.gameObject.SetActive(false);
        foreach(Vector2 existanceRange in crystal.existanceRanges){
          if(_game.GetCurrentTime() >= existanceRange[0] && _game.GetCurrentTime() <= existanceRange[1]){
            crystal.gameObject.SetActive(true);
          }
        }
      }
    }
}
