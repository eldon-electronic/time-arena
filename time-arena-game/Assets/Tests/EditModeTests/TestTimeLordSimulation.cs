using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestTimeLordSimulation: Tester
{
    public bool Authenticate() { return true; }

    [Test]
    public void TestSinglePlayerNoTimeTravel()
    {
        // Set some initial values.
        int playerID = 0;
        Vector3 pos = new Vector3(0, 0, 0);
        Quaternion rot = new Quaternion(0, 0, 0, 0);
        Constants.JumpDirection dir = Constants.JumpDirection.Static;

        // Run the simulation.
        TimeLord timeLord = new TimeLord(20);
        timeLord.Connect(playerID, true);
        timeLord.EnterReality(playerID);
        
        for (int i=0; i < 20; i++)
        {
            pos = new Vector3(i, i, i);
            rot = new Quaternion(i, i, i, i);
            PlayerState ps = new PlayerState(0, pos, rot, dir, false);
            timeLord.RecordState(ps);
            timeLord.Tick();
        }

        // Access the final resulting structures from TimeLord.
        Dictionary<int, PlayerState>[] states = timeLord.RevealPlayerStates(this);
        RealityManager realityManager = timeLord.RevealRealityManager(this);

        // Perform assertions on Player States.
        Assert.AreEqual(20, states.Length, "PlayerStates array does not have the correct length.");

        for (int i=0; i < 19; i++)
        {
            Assert.IsTrue(states[i] != null, $"State not stored at frame {i}.");
            Assert.AreEqual(1, states[i].Count, $"Incorrect number of states stored at frame {i}: {states[i].Count}.");
            Assert.IsTrue(states[i].ContainsKey(0), $"Tail 0 does not have a state stored at frame {i}.");
            
            PlayerState ps = states[i][0];
            Assert.AreEqual(0, ps.PlayerID, $"Player ID not recorded correctly at frame {i}.");
            Assert.AreEqual(0, ps.TailID, $"Tail ID not recorded correctly at frame {i}.");
            Assert.AreEqual(new Vector3(i, i, i), ps.Pos, $"Position not recorded correctly at frame {i}.");
            Assert.AreEqual(new Quaternion(i, i, i, i), ps.Rot, $"Rotation not recorded correctly at frame {i}.");
            Assert.AreEqual(dir, ps.JumpDirection, $"Jump direction not recorded correctly at frame {i}.");
        }

        // Perform assertions on Reality Manager.
        Dictionary<int, Reality> heads = realityManager.RevealHeads(this);
        Assert.AreEqual(1, heads.Count, "Incorrect number of realities.");
        Assert.IsTrue(heads.ContainsKey(0), "Reality Manager does not contain a reality for player 0.");
        
        Reality reality = heads[0];
        Assert.AreEqual(19, reality.PerceivedFrame, "Incorrect perceived frame.");
        Assert.AreEqual(0, reality.LastTailID, "Incorrect last tail ID.");

        List<int> writeFrames = reality.WriteFrames;
        Assert.AreEqual(1, writeFrames.Count, "Incorrect number of tail writer pointers.");
        Assert.AreEqual(19, writeFrames[0], "Incorrect tail writer pointer position.");

        Debug.Log("All assertions pass.");
    }

    [Test]
    public void TestSinglePlayerNoTimeTravelWithSimulator()
    {
        // Build and run the simulation.
        TimeSimulator sim = new TimeSimulator(20, 1, 120, false);
        sim.Run();

        // Access the final resulting structures from the Simulator.
        Dictionary<int, PlayerState>[] states = sim.RevealPlayerStates(this);
        RealityManager realityManager = sim.RevealRealityManager(this);

        // Perform assertions on Player States.
        Assert.AreEqual(20, states.Length, "PlayerStates array does not have the correct length.");

        for (int i=0; i < 19; i++)
        {
            Assert.IsTrue(states[i] != null, $"State not stored at frame {i}.");
            Assert.AreEqual(1, states[i].Count, $"Incorrect number of states stored at frame {i}: {states[i].Count}.");
            Assert.IsTrue(states[i].ContainsKey(0), $"Tail 0 does not have a state stored at frame {i}.");
            
            PlayerState ps = states[i][0];
            Assert.AreEqual(0, ps.PlayerID, $"Player ID not recorded correctly at frame {i}.");
            Assert.AreEqual(0, ps.TailID, $"Tail ID not recorded correctly at frame {i}.");
            Assert.AreEqual(new Vector3(i, i, i), ps.Pos, $"Position not recorded correctly at frame {i}.");
            Assert.AreEqual(new Quaternion(i, i, i, i), ps.Rot, $"Rotation not recorded correctly at frame {i}.");
            Assert.AreEqual(Constants.JumpDirection.Static, ps.JumpDirection, $"Jump direction not recorded correctly at frame {i}.");
        }

        // Perform assertions on Reality Manager.
        Dictionary<int, Reality> heads = realityManager.RevealHeads(this);
        Assert.AreEqual(1, heads.Count, "Incorrect number of realities.");
        Assert.IsTrue(heads.ContainsKey(0), "Reality Manager does not contain a reality for player 0.");
        
        Reality reality = heads[0];
        Assert.AreEqual(19, reality.PerceivedFrame, "Incorrect perceived frame.");
        Assert.AreEqual(0, reality.LastTailID, "Incorrect last tail ID.");

        List<int> writeFrames = reality.WriteFrames;
        Assert.AreEqual(1, writeFrames.Count, "Incorrect number of tail writer pointers.");
        Assert.AreEqual(19, writeFrames[0], "Incorrect tail writer pointer position.");

        Debug.Log("All assertions pass.");
    }

    [Test]
    public void TestSinglePlayerJumpBackOnce()
    {
        // Build and run the simulation.
        TimeSimulator sim = new TimeSimulator(50, 1, 5, false);
        sim.AddJump(0, 30, Constants.JumpDirection.Backward, 3);
        sim.Run();

        // Access the final resulting structures from the Simulator.
        Dictionary<int, PlayerState>[] states = sim.RevealPlayerStates(this);
        RealityManager realityManager = sim.RevealRealityManager(this);

        // Perform assertions on Player States.
        Assert.AreEqual(50, states.Length, "50 states should be stored.");
        
        // Check frame 0.
        Assert.AreEqual(2, states[0].Count);
        Assert.IsTrue(states[0].ContainsKey(0));
        Assert.IsTrue(states[0].ContainsKey(1));

        // Check frames 1 to 16.

        // Check frames 17 to 29.

        // Check frame 30.

        // Check frames 31 to 35.

        // Check frames 36 to 50.
    }
}
