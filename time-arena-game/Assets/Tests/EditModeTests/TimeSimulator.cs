using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TimeSimulator
{
    private class Player
    {
        public Queue<(int exitFrame, Constants.JumpDirection direction, int duration)> Jumps;
        public Constants.JumpDirection JumpDirection;
        public int FramesKeyPressedFor;

        public Player()
        {
            Jumps = new Queue<(int exitFrame, Constants.JumpDirection direction, int duration)>();
            JumpDirection = Constants.JumpDirection.Static;
            FramesKeyPressedFor = 0;
        }

        public void AddJump(int frame, Constants.JumpDirection direction, int duration)
        {
            Jumps.Enqueue((frame, direction, duration));
        }
    }

    private int _simulationLength;
    private List<Player> _players;
    private int _dissolveTime;

    public TimeSimulator(int length, int numPlayers, int dissolveTime)
    {
        _simulationLength = length;

        _players = new List<int>();
        for (int i=0; i < numPlayers; i++)
        {
            _players.Add(new Player(i));
        }

        _dissolveTime = dissolveTime;
    }

    public void AddJump(int playerID, int exitFrame, Constants.JumpDirection direction, int duration)
    {
        _players[playerID].AddJump(exitFrame, direction, duration);
    }

    public async void Run()
    {
        TimeLord timeLord = new TimeLord(_simulationLength);
        foreach (var player in _players)
        {
            timeLord.Connect(player);
            timeLord.EnterReality(player);
        }

        for (int frame=0; frame < _simulationLength; frame++)
        {
            timeLord.Tick();
            for (int id=0; id < _players.Count; id++)
            {
                // Starts jumping.
                if (_players[id].Jumps.ContainsKey(frame))
                {
                    _players[id].JumpDirection = _players[id].Jumps[frame].direction;
                }

                // Is currently jumping.
                else if (_players[id].JumpDirection != Constants.JumpDirection.Static)
                {
                    _players[id].FramesKeyPressedFor++;
                }

                // Has stopped their exit dissolve.
                if (_players[id].FramesKeyPressedFor > _dissolveTime)
                {

                }

                // Has entered a new reality.
                if (_players[id].FramesKeyPressedFor == _players[id])

                // Has stopped their entry dissolve.

                PlayerState ps = new PlayerState(
                    id,
                    new Vector3(frame, frame, frame),
                    new Quaternion(frame, frame, frame, frame),
                    Constants.JumpDirection.Static);
                timeLord.RecordState(ps);
            }
        }
    }
}
