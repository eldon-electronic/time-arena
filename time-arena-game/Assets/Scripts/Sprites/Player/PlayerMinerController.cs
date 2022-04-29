using Photon.Pun;
using UnityEngine;

public class PlayerMinerController : PlayerController
{
	[SerializeField] private GameObject _minerDevice;
	public int Score;

	protected override void SetActive()
	{
        _mesh.SetActive(!_view.IsMine);
        _minerDevice.SetActive(true);
    }

    protected override void SetTeam()
	{
		// TODO: Set the team in the menu before loading the pregame scene.
		Team = Constants.Team.Miner;
	}

    public override void SetSceneController(SceneController sceneController)
    {
        _sceneController = sceneController;
        _sceneController.Register(this);
    }

	[PunRPC]
	public void RPC_incrementScore()
	{
		Score++;
		_sceneController.OffsetScore(ID, 1);
	}

	[PunRPC]
	public void RPC_getGrabbed()
	{
		int offset = Score / 2;
		Score -= offset;
		_sceneController.OffsetScore(ID, -offset);
		//TODO: respawn?
	}
}
