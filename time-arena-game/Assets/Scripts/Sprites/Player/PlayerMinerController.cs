using Photon.Pun;
using UnityEngine;

public class PlayerMinerController : PlayerController
{
	[SerializeField] private GameObject _minerDevice;
	[SerializeField] protected HudScore _hudScore;
	[SerializeField] private TimeConn _timeConn;
	[SerializeField] private AudioSource _soundSource;
	[SerializeField] private AudioSource _spatialSource;
	[SerializeField] private AudioClip _collectionClip;
	[SerializeField] private AudioClip _wilhelmScream;

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

	public override void IncrementScore()
	{
		_view.RPC("RPC_incrementScore", RpcTarget.All);
		_view.RPC("RPC_offsetScore", RpcTarget.All, 1);
		_soundSource.PlayOneShot(_collectionClip);
		_hudScore.SetYourScore(Score);
	}

	public void PlayCollectSFX() {

	}

	[PunRPC]
	public void RPC_getGrabbed()
	{
		if (_view.IsMine)
		{
			_spatialSource.PlayOneShot(_wilhelmScream);
			int offset = Score / 2;
			Score -= offset;
			_view.RPC("RPC_offsetScore", RpcTarget.All, -offset);
			_hudScore.SetYourScore(Score);
			_timeConn.ForceJump();
		}
	}

	[PunRPC]
	public void RPC_offsetScore(int offset) { _sceneController.OffsetScore(offset); }
}
