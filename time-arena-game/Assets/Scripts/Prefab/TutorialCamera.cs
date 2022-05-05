using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCamera : MonoBehaviour
{
    [SerializeField] private GameObject _camera;
    public static event Action collectableAppears;
    public static event Action collectableDisappears;
    public static event Action obstacleGrows;
    public static event Action faceYourself;
    public static event Action checkTracker;
    public static event Action goodLuck;
    public static event Action endTutorial;

    public void CollectableAppears() { collectableAppears?.Invoke(); }

    public void CollectableDisappears() { collectableDisappears?.Invoke(); }

    public void ObstacleGrows() { obstacleGrows?.Invoke(); }

    public void FaceYourself() { faceYourself?.Invoke(); }

    public void CheckTracker() { checkTracker?.Invoke(); }

    public void GoodLuck() { goodLuck?.Invoke(); }

    public void EndTutorial()
    {
        endTutorial?.Invoke();
    }
}