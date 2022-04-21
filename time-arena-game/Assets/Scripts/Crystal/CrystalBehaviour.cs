using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static System.Math;

public class CrystalBehaviour : MonoBehaviour
{
    //attributes relating to anim/aesthetics
    public Material overlay;
    public float initial_wave = 0;
    public float t = 0;

    //attributes for crystalmanager access
    private CrystalManager cm;
    public int ID;
    private SceneController sceneController;

    //attributes defining crystal state
    public Vector2 existanceRange = new Vector2(5f, 10f);
    public bool isCollected = false; //if isCollected is true - there is no instance of the crystal at any time


    // Start is called before the first frame
    void Start(){
      initial_wave = Random.Range(5f, 10f);
      sceneController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<SceneController>();
      cm = sceneController.gameObject.GetComponent<CrystalManager>();

      //on instantiation, add self to crystal list in cm
      //give self id according to position in list (syncing doesnt matter, only uniqueness and size)
      ID = cm.crystals.Count;
      cm.crystals.Add(this);
      UpdateAnim();
    }

    // Update is called once per frame
    void Update()
    {
      UpdateAnim();
    }

    //update aesthetics
    public void UpdateAnim(){
      t += Time.deltaTime;
      float offsetY = (float)(0.01 * Sin(t));
      gameObject.transform.Translate(new Vector3(0.0f, offsetY, 0.0f));
      transform.Rotate(0.0f, 30f*Time.deltaTime, 0.0f, Space.Self);
      overlay.SetFloat("Wave_Incr", t);

      //zoom into and out of existance
      TimeLord timelord = sceneController.GetTimeLord();
      float percievedTime = (float)(timelord.GetMyPercievedFrame()) / Constants.FrameRate;
      float closestBorderOFExistance = Min(Abs(percievedTime - existanceRange[0]), Abs(percievedTime - existanceRange[1]));
      float animLength = 2.0f;
      float size = Min(closestBorderOFExistance, t);
      if(size > animLength){
        setScale(1.0f);
      } else {
        setScale(size/animLength);
      }
    }

    //setScale
    void setScale(float a){
      transform.localScale = new Vector3(a, a, a);
    }

    //called upon player collision
    // - crystal will be set to inactive in following frame so coroutine outsourced to cm
    [PunRPC]
    void RPC_Collect(int viewID){
      existanceRange = new Vector2(-1f, -1f);
      isCollected = true;
      PhotonView viewOfCollector = PhotonView.Find(viewID);
      if(viewOfCollector.IsMine){
        sceneController.IncrementPlayerScore();
      } else {
        sceneController.IncrementMinerScore();
      }
      if(PhotonNetwork.IsMasterClient){
        cm.StartCoroutine(cm.Respawn(ID));
      }
    }

    //set existance range (period of time when crystal exists in game)
    //i.e. spawn a new crystal (called by coroutine after x seconds)
    [PunRPC]
  	void RPC_setExistanceRange(Vector2 newRange)
  	{
      existanceRange = newRange;
      isCollected = false;
    }
}
