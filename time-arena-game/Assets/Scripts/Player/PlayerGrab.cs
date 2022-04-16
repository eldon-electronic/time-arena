using System;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    [SerializeField] private Animator PlayerAnim;
	[SerializeField] private LayerMask GrabMask;
    [SerializeField] private PlayerController _player;
	private float _grabCheckRadius = 1f;
	private bool _damageWindow = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) Grab();
    }

    private void Grab()
	{
		// If grabbing, check for intersection with player.
		if (!_damageWindow)
		{
			Collider[] playersGrab = Physics.OverlapSphere(transform.position, _grabCheckRadius, GrabMask);
			foreach (var playerGotGrab in playersGrab)
			{
				// Call grabplayer function on that player.
				PlayerController targetPlayer = playerGotGrab.GetComponent<PlayerController>();
				if (_player.Team == Constants.Team.Guardian && 
					targetPlayer.Team == Constants.Team.Miner)
				{
					// TODO: grab the miner.
				}
			}
			PlayerAnim.SetBool("isGrabbing", true);
		}
	}

    // Function called by animation to enable player to grab others.
	public void StartGrabbing()
	{
		_damageWindow = true;
	}

	// Function called by animation to disable player to grab others.
	public void StopGrabbing()
	{
		_damageWindow = false;
		PlayerAnim.SetBool("isGrabbing", false);
	}
}
