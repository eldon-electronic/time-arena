using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// TODO: Might be able to get away with not storing PlayerID? It's not used for anything atm...
public class PlayerState
{
    public readonly int PlayerID;
    public int TailID;
    public readonly Vector3 Pos;
    public readonly Quaternion Rot;
    public readonly Constants.JumpDirection JumpDirection;
    public readonly bool JumpingOut;

    public PlayerState(int playerID, Vector3 pos, Quaternion rot, Constants.JumpDirection dir, bool jumpingOut)
    {
        PlayerID = playerID;
        Pos = pos;
        Rot = rot;
        JumpDirection = dir;
        JumpingOut = jumpingOut;
    }
}
