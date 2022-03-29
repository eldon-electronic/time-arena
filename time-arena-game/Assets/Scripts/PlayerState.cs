using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// TODO: need to store the tail ID, not just the player ID in case they have multiple tails in the same frame
public class PlayerState
{
    public readonly int PlayerID;
    public int TailID;
    public readonly Vector3 Pos;
    public readonly Quaternion Rot;
    public readonly Constants.JumpDirection JumpDirection;

    public PlayerState(int playerID, Vector3 pos, Quaternion rot, Constants.JumpDirection dir)
    {
        PlayerID = playerID;
        Pos = pos;
        Rot = rot;
        JumpDirection = dir;
    }
}
