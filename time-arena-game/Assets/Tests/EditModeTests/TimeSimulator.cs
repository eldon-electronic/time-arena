using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

public class TimeAssertionChecker
{
    private class TimeAssertion
    {
        public int TailID;
        public (int, int) GameTimeInterval;
        public (int, int) RealTimeInterval;
        public int jumpFrame;
        public Constants.JumpDirection direction;
    }

    private TimeLord _timeLord;
    private List<TimeAssertion> _assertions;

    public TimeAssertionChecker(TimeLord timeLord, int length, int dissolveTime,
        List<Queue<(int exitFrame, Constants.JumpDirection direction, int duration)>> players)
    {
        _timeLord = timeLord;

        for (int i=0; i < players.Count; i++)
        {
            TimeAssertion ta = new TimeAssertion();
            ta.TailID = i * 100;
            ta.GameTimeInterval = (0, length);
            ta.RealTimeInterval = (0, length);
            ta.jumpFrame = -1;

            if (players[i].Count == 0)
            {
                _assertions.Add(ta);
            }

            else
            {
                foreach (var jump in players[i])
                {
                    ta.GameTimeInterval.Item2 = jump.exitFrame + dissolveTime;
                }
            }
        }
    }

    public void RunAssertions()
    {

    }
}

public class TimeSimulator: Tester
{
    private class Player
    {
        public Queue<(int exitFrame, Constants.JumpDirection direction, int duration)> Jumps;
        public Constants.JumpDirection JumpDirection;
        public int JumpTimer;
        public int DissolveOutTimer;
        public int DissolveInTimer;
        public int FramesKeyPressedFor;

        public Player()
        {
            Jumps = new Queue<(int exitFrame, Constants.JumpDirection direction, int duration)>();
            JumpDirection = Constants.JumpDirection.Static;
            JumpTimer = -1;
            DissolveOutTimer = -1;
            DissolveInTimer = -1;
        }

        public void AddJump(int frame, Constants.JumpDirection direction, int duration)
        {
            Jumps.Enqueue((frame, direction, duration));
        }
    }

    private int _simulationLength;
    private List<Player> _players;
    private int _dissolveTime;
    private TimeLord _timeLord;
    private bool _writeFinalStates;

    // All time values are measured in frames.
    public TimeSimulator(int length, int numPlayers, int dissolveTime, bool writeFinalStates)
    {
        _simulationLength = length;

        _players = new List<Player>();
        for (int i=0; i < numPlayers; i++)
        {
            _players.Add(new Player());
        }

        _dissolveTime = dissolveTime;
        _timeLord = new TimeLord(_simulationLength);
        _writeFinalStates = writeFinalStates;
    }

    public bool Authenticate() { return true; }

    // Add a jump to the queue. This will simulate a time jump for the given player,
    // starting from the exitFrame in the jump direction for the number of frames given by duration.
    public void AddJump(int playerID, int exitFrame, Constants.JumpDirection direction, int duration)
    {
        _players[playerID].AddJump(exitFrame, direction, duration);
    }

    public void CreateAssertionChecker()
    {

    }

    // Run the simulation.
    public void Run()
    {
        for (int i=0; i < _players.Count; i++)
        {
            _timeLord.Connect(i, i == 0);
            _timeLord.EnterReality(i);
        }

        for (int frame=0; frame < _simulationLength; frame++)
        {
            for (int id=0; id < _players.Count; id++)
            {
                // 1 Animation events happen first I believe.

                // Animation finished dissolving out on this frame.
                if (_players[id].DissolveOutTimer == 0)
                {
                    _timeLord.LeaveReality(id);
                }

                // Animation finished dissolving in on this frame.
                if (_players[id].DissolveInTimer == 0)
                {
                    _players[id].JumpDirection = Constants.JumpDirection.Static;
                    _players[id].Jumps.Dequeue();
                }


                // 2 PlayerController:Update() -> KeyControl()

                // Pressed jump key down on this frame.
                if (_players[id].Jumps.Count > 0)
                {
                    (int exitFrame, Constants.JumpDirection direction, int duration) currentJump = _players[id].Jumps.Peek();
                    if (currentJump.exitFrame == frame && _players[id].DissolveInTimer == -1)
                    {
                        _players[id].JumpDirection = currentJump.direction;
                        _players[id].JumpTimer = currentJump.duration;
                        _players[id].DissolveOutTimer = _dissolveTime;
                    }
                }

                // Released jump key on this frame.
                if (_players[id].JumpTimer == 0)
                {
                    _timeLord.EnterReality(id);
                    _players[id].DissolveInTimer = _dissolveTime;
                }


                // 3. PlayerController:Update()

                // Record state.
                PlayerState ps = new PlayerState(
                    id,
                    new Vector3(frame, frame, frame),
                    new Quaternion(frame, frame, frame, frame),
                    _players[id].JumpDirection,
                    _players[id].DissolveOutTimer == _dissolveTime);
                _timeLord.RecordState(ps);

                // Time travel and progress dissolve animations.
                if (_players[id].JumpTimer > -1)
                {
                    _timeLord.TimeTravel(id, _players[id].JumpDirection);
                    _players[id].JumpTimer--;
                }
                if (_players[id].DissolveOutTimer > -1) _players[id].DissolveOutTimer--;
                if (_players[id].DissolveInTimer > -1) _players[id].DissolveInTimer--;
            }

            // 4. GameController:Update()
            // Beware that this may happen before PlayerController:Update().
            // I don't believe this will be a problem until the final frame of the game.
            _timeLord.Tick();
        }

        if (_writeFinalStates)
        {
            _timeLord.SnapshotStates("StateSnapshot.txt");
        }
    }

    public Dictionary<int, PlayerState>[] RevealPlayerStates(Tester tester)
    {
        if (tester.Authenticate()) return _timeLord.RevealPlayerStates(this);
        else throw new InvalidOperationException("Must be a Tester to call this method.");
    }

    public RealityManager RevealRealityManager(Tester tester)
    {
        if (tester.Authenticate()) return _timeLord.RevealRealityManager(this);
        else throw new InvalidOperationException("Must be a Tester to call this method.");
    }
}
