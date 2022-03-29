using System.Collections;
using System.Collections.Generic;

public static class Constants
{
    public static int GameLength = 5 * 60;
    public static int FrameRate = 30;
    public static int MaxPlayers = 10;
    public static int MinTimeSnapDistance = 10 * FrameRate;
    public static int TimeTravelVelocity = 10;
    public static int AnimationFrames = 3 * FrameRate;

    public enum Team
    {
        Guardian,
        Miner
    }

    public enum JumpDirection
    {
        Static,
        Forward,
        Backward
    }
}
