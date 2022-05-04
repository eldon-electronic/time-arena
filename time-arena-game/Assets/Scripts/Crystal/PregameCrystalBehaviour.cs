using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PregameCrystalBehaviour: CrystalBehaviour
{
  // ------------ UNITY METHODS ------------

  protected override void Start()
  {
    ExistanceRange = new Vector2(1f, 60f);
    ExistanceLength = 60f;
    _sceneController = FindObjectOfType<SceneController>();
    UpdateAnim();
  }

  // Called upon player collision.
  // Crystal will be set to inactive in following frame so coroutine outsourced to cm.
  [PunRPC]
  protected override void RPC_Collect()
  {
    Destroy(gameObject);
    Destroy(this);
  }
}
