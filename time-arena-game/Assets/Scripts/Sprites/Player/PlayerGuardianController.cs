using Photon.Pun;
using UnityEngine;

public class PlayerGuardianController : PlayerController
{
    [SerializeField] protected HudScore _hudScore;
    
    public override void SetActive(bool _isPreGame)
    {
        _mesh.SetActive(!_view.IsMine || _isPreGame);
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
