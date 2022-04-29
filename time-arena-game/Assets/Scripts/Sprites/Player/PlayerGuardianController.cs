using UnityEngine;

public class PlayerGuardianController : PlayerController
{
    protected override void SetActive()
    {
        _mesh.SetActive(!_view.IsMine);
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
}
