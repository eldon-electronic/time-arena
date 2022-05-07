using System;
using UnityEngine;

public class Tutorial2: MonoBehaviour
{
    [SerializeField] private HudTutorial _tutorialHud;
    [SerializeField] private PlayerController _player;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private HudKeyPanel _hudKeyPanel;

    void OnEnable()
    {
        TutorialCamera.collectableAppears += OnCollectableAppears;
        TutorialCamera.collectableDisappears += OnCollectableDisappears;
        TutorialCamera.obstacleGrows += OnObstacleGrows;
        TutorialCamera.faceYourself += OnFaceYourself;
        TutorialCamera.checkTracker += OnCheckTracker;
        TutorialCamera.hidePlayer += OnHidePlayer;
        TutorialCamera.showUI += OnShowUI;
        TutorialCamera.showBackArrow += OnShowBackArrow;
        TutorialCamera.showForwardArrow += OnShowForwardArrow;
        TutorialCamera.showTimeline += OnShowTimeline;
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
        TutorialCamera.hidePlayer -= OnHidePlayer;
        TutorialCamera.showUI -= OnShowUI;
        TutorialCamera.showBackArrow -= OnShowBackArrow;
        TutorialCamera.showForwardArrow -= OnShowForwardArrow;
        TutorialCamera.showTimeline -= OnShowTimeline;
        TutorialCamera.goodLuck -= OnGoodLuck;
        TutorialCamera.endTutorial -= OnEndTutorial;
    }

    void Start()
    {
        _playerCamera.enabled = false;
        // _tutorialHud.SetVisibilityUI(false);
    }

    private void OnCollectableAppears()
    {
        if (_player.Team == Constants.Team.Miner)
        {
            _tutorialHud.SetMessage("This the collectable crystal,you should run through it to collect.");
        }
        else
        {
            _tutorialHud.SetMessage("This is a Miner!");
        }
    }

    private void OnCollectableDisappears()
    {
        if (_player.Team == Constants.Team.Miner)
        {
            _tutorialHud.SetMessage("It only appears at certain times,you should time travel to find them");
        }
        else
        {
            _tutorialHud.SetMessage("Click to catch them and steal their crystals!");
        }
    }

    private void OnObstacleGrows()
    {
        _tutorialHud.SetMessage("These crystals are obstacles you should time travel to pass them.");
    }

    private void OnFaceYourself()
    {
        if (_player.Team == Constants.Team.Miner)
        {
            _tutorialHud.SetMessage("This is you Miner.");
        }
        else
        {
            _tutorialHud.SetMessage("This is you Guardian.");
        }
    }

    private void OnCheckTracker()
    {
        if (_player.Team == Constants.Team.Miner)
        {
            _tutorialHud.SetMessage("And this is your tracker which helps you to find the nearest crystal.");
        }
        else
        {
            _tutorialHud.SetMessage("Let's start playing.");
        }
    }

    private void OnHidePlayer()
    {
        _player.SetActive(false);
    }
    private void OnShowUI()
    {
        _tutorialHud.SetVisibilityUI(true);
    }
    private void OnShowBackArrow()
    {
        _tutorialHud.SetVisibilityArrow("backJump",true);
        _tutorialHud.SetMessage("You can only travel backwards when orange icon starts spinning.");
    }  
    private void OnShowForwardArrow()
    {
        _tutorialHud.SetVisibilityArrow("forwardJump",true);
        _tutorialHud.SetMessage("You can only travel forwards when blue icon starts spinning.");
    }
    private void OnShowTimeline()
    {
        _tutorialHud.SetVisibilityArrow("timeline",true);
        _tutorialHud.SetMessage("This is the time line which shows where you are at in time\nYou're icon is the biggest one.");
    }

    private void OnGoodLuck()
    {
        _tutorialHud.SetMessage("Good luck!");
    }

    private void OnEndTutorial()
    {
        Debug.Log("OnEndTutorial called");
        _playerCamera.enabled = true;
        _tutorialHud.SetActive(false);
        _hudKeyPanel.SetActive(true);
        Destroy(this);
    }
}
