using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CrystalManager : MonoBehaviour
{

  [SerializeField] private GameObject spawnHolder;
  [SerializeField] private GameObject crystalPrefab;
  [SerializeField] private GameController game;

  private Transform[] spawnPoints;
  public List<CrystalBehaviour> crystals = new List<CrystalBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
      spawnPoints = new Transform[spawnHolder.transform.childCount];
      int i = 0;
      foreach(Transform child in spawnHolder.transform){
        spawnPoints[i++] = child;
      }

      foreach(Transform spawnPoint in spawnPoints){
        PhotonNetwork.Instantiate(crystalPrefab.name, spawnPoint.position, Quaternion.identity);
      }
    }

    // Update is called once per frame
    void Update()
    {
      foreach(CrystalBehaviour crystal in crystals){
        float percievedTime = (game._timeLord.GetYourPosition()*game._timeLord._totalFrames) / Constants.FrameRate;
        if(percievedTime >= crystal.existanceRange[0] && percievedTime <= crystal.existanceRange[1]){
          crystal.gameObject.SetActive(true);//set layers here
        } else {
          crystal.gameObject.SetActive(false);//set layers here
        }
      }
    }


    public IEnumerator Respawn(int id){
      float newExistance = Random.Range(0.0f, (game._timeLord.GetTimeProportion()*game._timeLord._totalFrames)/Constants.FrameRate);
      Vector2 newExistanceRange = new Vector2(newExistance, newExistance + 10f );
      yield return new WaitForSeconds(1.0f);
      crystals[id].gameObject.GetComponent<PhotonView>().RPC("RPC_setExistanceRange", RpcTarget.All, newExistanceRange);
      Debug.Log("check2");
    }
}
