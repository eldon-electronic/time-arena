using Photon.Pun;
using UnityEngine;

public class PlayerGuardianController : PlayerController
{
    [SerializeField] protected HudScore _hudScore;
    [SerializeField] private GameObject _upperArm;
    [SerializeField] private GameObject _lowerArm;
    [SerializeField] private GameObject _hand;
    
    public override void SetActive(bool _isPreGame)
    {
        _mesh.SetActive(!_view.IsMine || _isPreGame);
        _upperArm.SetActive(true);
        _lowerArm.SetActive(true);
        _hand.SetActive(true);
        // TODO: Fix this; setting the arm active isn't working right now.
    }

	protected override void SetTeam()
	{
		// TODO: Set the team in the menu before loading the pregame scene.
		Team = Constants.Team.Guardian;
	}

    public override void SetSceneController(SceneController sceneController)
    {
        _sceneController = sceneController;
        _sceneController.Register(this);
    }

    public override void IncrementScore()
	{
		_view.RPC("RPC_incrementScore", RpcTarget.All);
		_hudScore.SetYourScore(Score);
	}
}
