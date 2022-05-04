using System;
using UnityEngine;

public class Tutorial2: MonoBehaviour
{
    void OnEnable()
    {
        TutorialCamera.collectableAppears += OnCollectableAppears;
        TutorialCamera.collectableDisappears += OnCollectableDisappears;
        TutorialCamera.obstacleGrows += OnObstacleGrows;
        TutorialCamera.faceYourself += OnFaceYourself;
        TutorialCamera.checkTracker += OnCheckTracker;
        TutorialCamera.goodLuck += OnGoodLuck;
        TutorialCamera.endTutorial += OnEndTutorial;
    }

    void OnDisable()
    {
        TutorialCamera.collectableAppears -= OnCollectableAppears;
        TutorialCamera.collectableDisappears -= OnCollectableDisappears;
        TutorialCamera.obstacleGrows -= OnObstacleGrows;
        TutorialCamera.faceYourself -= OnFaceYourself;
        TutorialCamera.checkTracker -= OnCheckTracker;
        TutorialCamera.goodLuck -= OnGoodLuck;
        TutorialCamera.endTutorial -= OnEndTutorial;
    }

    private void OnCollectableAppears()
    {

    }

    private void OnCollectableDisappears()
    {

    }

    private void OnObstacleGrows()
    {

    }

    private void OnFaceYourself()
    {

    }

    private void OnCheckTracker()
    {

    }

    private void OnGoodLuck()
    {

    }

    private void OnEndTutorial()
    {

    }
}
