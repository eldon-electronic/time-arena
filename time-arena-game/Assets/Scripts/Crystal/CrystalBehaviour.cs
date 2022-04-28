using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static System.Math;

public class CrystalBehaviour : MonoBehaviour
{
    // Attributes relating to anim/aesthetics.
    public Material overlay;
    public float initial_wave = 0;
    public float t = 0;

    // Attributes for crystalmanager access.
    private CrystalManager cm;
    public int ID;
    private SceneController sceneController;

    // Attributes defining crystal state.
    public Vector2 existanceRange = new Vector2(5f, 10f);
    // If isCollected is true there is no instance of the crystal at any time.
    public bool isCollected = false;

    void Start()
    {
      initial_wave = Random.Range(5f, 10f);
      sceneController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<SceneController>();
      cm = sceneController.gameObject.GetComponent<CrystalManager>();

      // On instantiation, add self to crystal list in cm.
      // Give self id according to position in list (syncing doesnt matter, only uniqueness and size).
      ID = cm.crystals.Count;
      cm.crystals.Add(this);
      UpdateAnim();
    }

    void Update()
    {
      UpdateAnim();
    }

    // Update aesthetics.
    public void UpdateAnim(){
      t += Time.deltaTime;
      float offsetY = (float)(0.01 * Sin(t));
      gameObject.transform.Translate(new Vector3(0.0f, offsetY, 0.0f));
      transform.Rotate(0.0f, 30f*Time.deltaTime, 0.0f, Space.Self);
      overlay.SetFloat("Wave_Incr", t);

      // Zoom into and out of existance
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

    void setScale(float a){
      transform.localScale = new Vector3(a, a, a);
    }

    // Called upon player collision.
    // Crystal will be set to inactive in following frame so coroutine outsourced to cm.
    [PunRPC]
    void RPC_Collect()
    {
      Debug.Log("RPC collect called");
      existanceRange = new Vector2(-1f, -1f);
      isCollected = true;
      if (PhotonNetwork.IsMasterClient)
      {
        cm.StartCoroutine(cm.Respawn(ID));
      }
    }

    // Set existance range (period of time when crystal exists in game).
    // i.e. spawn a new crystal (called by coroutine after x seconds).
    [PunRPC]
  	void RPC_setExistanceRange(Vector2 newRange)
  	{
      existanceRange = newRange;
      isCollected = false;
    }
}
