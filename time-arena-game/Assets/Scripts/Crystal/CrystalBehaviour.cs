using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static System.Math;

public class CrystalBehaviour : MonoBehaviour
{
    public Material overlay;
    public float initial_wave = 0;
    private float t = 0;

    private CrystalManager cm;
    public int ID;

    public Vector2 existanceRange = new Vector2(5f, 10f);
    public bool isCollected = false;

    public void Collect(){
      existanceRange = new Vector2(-1f, -1f);
      isCollected = true;
      if(PhotonNetwork.IsMasterClient){
        cm.StartCoroutine(cm.Respawn(ID));
      }
    }

    void Start(){
      initial_wave = Random.Range(5f, 10f);
      cm = GameObject.FindGameObjectsWithTag("TimeLord")[0].GetComponent<CrystalManager>();
      ID = cm.crystals.Count;
      cm.crystals.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
      UpdateAnim();
    }

    void UpdateAnim(){
      t += Time.deltaTime;
      float offsetY = (float)(0.01 * Sin(t));
      gameObject.transform.Translate(new Vector3(0.0f, offsetY, 0.0f));
      transform.Rotate(0.0f, 30f*Time.deltaTime, 0.0f, Space.Self);
      overlay.SetFloat("Wave_Incr", t);
    }

    [PunRPC]
  	void RPC_setExistanceRange(Vector2 newRange)
  	{
      Debug.Log(newRange);
      existanceRange = newRange;
      isCollected = false;
    }
}
