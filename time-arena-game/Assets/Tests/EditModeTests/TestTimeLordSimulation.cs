using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestTimeLordSimulation
{

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
        Dictionary<int, PlayerState>[] states = timeLord.RevealPlayerStates();
        RealityManager realityManager = timeLord.RevealRealityManager();
        Dictionary<int, List<int>> tailCreations = timeLord.RevealTailCreations();

        // Perform assertions on Player States.
        Assert.AreEqual(20, states.Length, "PlayerStates array does not have the correct length.");

        for (int i=0; i < 20; i++)
        {
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
        Dictionary<int, Reality> heads = realityManager.RevealHeads();
        Assert.AreEqual(1, heads.Count, "Incorrect number of realities.");
        Assert.IsTrue(heads.ContainsKey(0), "Reality Manager does not contain a reality for player 0.");
        
        Reality reality = heads[0];
        Assert.AreEqual(20, reality.PerceivedFrame, "Incorrect perceived frame.");
        Assert.AreEqual(0, reality.LastTailID, "Incorrect last tail ID.");

        List<int> writeFrames = reality.WriteFrames;
        Assert.AreEqual(1, writeFrames.Count, "Incorrect number of tail writer pointers.");
        Assert.AreEqual(20, writeFrames[0], "Incorrect tail writer pointer position.");

        // Perform assertions on Tail Creations.
        Assert.AreEqual(1, tailCreations.Count, "Incorrect number of tail creation frames.");
        Assert.IsTrue(tailCreations.ContainsKey(0), "Tail Creations does not contain an entry for frame 0.");
        Assert.AreEqual(1, tailCreations[0].Count, "Incorrect number of tail creations on frame 0.");
        Assert.AreEqual(0, tailCreations[0][0], "Incorrect tail ID created on frame 0.");

        Debug.Log("All assertions pass.");
    }

    [Test]
    public void TestSinglePlayerNoTimeTravelWithSimulator()
    {
        // Build and run the simulation.
        TimeSimulator sim = new TimeSimulator(20, 1, 120, false);
        sim.Run();

        // Access the final resulting structures from the Simulator.
        Dictionary<int, PlayerState>[] states = sim.RevealPlayerStates();
        RealityManager realityManager = sim.RevealRealityManager();
        Dictionary<int, List<int>> tailCreations = sim.RevealTailCreations();

        // Perform assertions on Player States.
        Assert.AreEqual(20, states.Length, "PlayerStates array does not have the correct length.");

        for (int i=0; i < 20; i++)
        {
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
        Dictionary<int, Reality> heads = realityManager.RevealHeads();
        Assert.AreEqual(1, heads.Count, "Incorrect number of realities.");
        Assert.IsTrue(heads.ContainsKey(0), "Reality Manager does not contain a reality for player 0.");
        
        Reality reality = heads[0];
        Assert.AreEqual(20, reality.PerceivedFrame, "Incorrect perceived frame.");
        Assert.AreEqual(0, reality.LastTailID, "Incorrect last tail ID.");

        List<int> writeFrames = reality.WriteFrames;
        Assert.AreEqual(1, writeFrames.Count, "Incorrect number of tail writer pointers.");
        Assert.AreEqual(20, writeFrames[0], "Incorrect tail writer pointer position.");

        // Perform assertions on Tail Creations.
        Assert.AreEqual(1, tailCreations.Count, "Incorrect number of tail creation frames.");
        Assert.IsTrue(tailCreations.ContainsKey(0), "Tail Creations does not contain an entry for frame 0.");
        Assert.AreEqual(1, tailCreations[0].Count, "Incorrect number of tail creations on frame 0.");
        Assert.AreEqual(0, tailCreations[0][0], "Incorrect tail ID created on frame 0.");

        Debug.Log("All assertions pass.");
    }

    [Test]
    public void TestSinglePlayerNoTimeTravelOutput()
    {
        // Build and run the simulation.
        TimeSimulator sim = new TimeSimulator(20, 1, 120, true);
        sim.Run();

        Debug.Log("Wrote to file.");
    }
}
