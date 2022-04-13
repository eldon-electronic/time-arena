using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class CrystalBehaviour : MonoBehaviour
{
    public Material overlay;
    public float initial_wave = 0;
    private float t = 0;

    public Vector2 existanceRange;
    public GameController _game;

    void Start(){
      _game = GameObject.FindGameObjectsWithTag("TimeLord")[0].GetComponent<GameController>();
      initial_wave = Random.Range(0f, 100f);
      existanceRange = new Vector2(_game.GetElapsedTime(), _game.GetElapsedTime()+100);
    }

    // Update is called once per frame
    void Update()
    {
      if(_game.GetCurrentTime() >= existanceRange[0] && _game.GetCurrentTime() <= existanceRange[1]){
        gameObject.SetActive(true); //shouldnt happen here
      } else {
        gameObject.SetActive(false);
      }
      t += Time.deltaTime;
      float offsetY = (float)(0.01 * Sin(t+initial_wave));
      gameObject.transform.Translate(new Vector3(0.0f, offsetY, 0.0f));
      transform.Rotate(0.0f, 30f*Time.deltaTime, 0.0f, Space.Self);
      overlay.SetFloat("Wave_Incr", t+initial_wave);
    }
}
