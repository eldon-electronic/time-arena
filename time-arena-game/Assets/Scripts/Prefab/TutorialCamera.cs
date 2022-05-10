using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCamera : MonoBehaviour
{
    public static event Action collectableAppears;
    public static event Action collectableDisappears;
    public static event Action obstacleGrows;
    public static event Action faceYourself;
    public static event Action checkTracker;
    public static event Action hidePlayer;
    public static event Action showUI;
    public static event Action showBackArrow;
    public static event Action showForwardArrow;
    public static event Action showTimeline;
    public static event Action showScore;    
    public static event Action goodLuck;
    public static event Action endTutorial;

    public void CollectableAppears() { collectableAppears?.Invoke(); }
    public void CollectableDisappears() { collectableDisappears?.Invoke(); }
    public void ObstacleGrows() { obstacleGrows?.Invoke(); }
    public void FaceYourself() { faceYourself?.Invoke(); }
    public void CheckTracker() { checkTracker?.Invoke(); }
    public void HidePlayer() { hidePlayer?.Invoke(); }
    public void ShowUI() { showUI?.Invoke(); }
    public void ShowBackArrow() { showBackArrow?.Invoke(); }
    public void ShowForwardArrow() { showForwardArrow?.Invoke(); }
    public void ShowTimeline() { showTimeline?.Invoke(); }
    public void ShowScore() { showScore?.Invoke(); }
    public void GoodLuck() { goodLuck?.Invoke(); }
    public void EndTutorial()
    {
        Debug.Log("EndTutorial called");
        endTutorial?.Invoke();
        Destroy(gameObject);
        Destroy(this);
    }
}