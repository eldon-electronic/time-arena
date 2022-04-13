using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static System.Math;

public class CrystalBehaviour : MonoBehaviour
{
    [SerializeField] private Material overlay;
    [SerializeField] private GameController _game;

    private float initial_wave = 0;
    private float t = 0;
    public int ID;

    private CrystalManager cm;
    public List<Vector2> existanceRanges;

    void Awake(){
      _game = GameObject.FindGameObjectsWithTag("TimeLord")[0].GetComponent<GameController>();
      cm = _game.gameObject.GetComponent<CrystalManager>();
      initial_wave = Random.Range(0f, 100f);
      existanceRanges.Add(new Vector2(0, 10));
    }

    void OnPhotonInstantiate(PhotonMessageInfo info){
      ID = cm.addCrystal(this);
    }

    // Update is called once per frame
    void Update()
    {
      updateAnim();
    }

    void updateAnim(){
      t += Time.deltaTime;
      float offsetY = (float)(0.01 * Sin(t+initial_wave));
      gameObject.transform.Translate(new Vector3(0.0f, offsetY, 0.0f));
      transform.Rotate(0.0f, 30f*Time.deltaTime, 0.0f, Space.Self);
      overlay.SetFloat("Wave_Incr", t+initial_wave);
    }
}
