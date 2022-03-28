using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestTimeLordSimulation
{

    [Test]
    public void TestOnePlayerNoTimeTravel()
    {
        // Set some initial values.
        int playerID = 0;
        Vector3 pos = new Vector3(0, 0, 0);
        Quaternion rot = new Quaternion(0, 0, 0, 0);
        Constants.JumpDirection dir = Constants.JumpDirection.Static;

        // Run the simulation.
        TimeLord timeLord = new TimeLord(20);
        timeLord.Connect(playerID, true);
        
        for (int i=0; i < 20; i++)
        {
            pos = new Vector3(i, i, i);
            rot = new Quaternion(i, i, i, i);
            PlayerState ps = new PlayerState(0, pos, rot, dir, false);
            timeLord.RecordState(ps);
            timeLord.Tick();
        }

        // Access the final resulting structure from TimeLord.
        Dictionary<int, PlayerState>[] states = timeLord.RevealPlayerStates();

        // Perform assertions.
        Assert.AreEqual(20, states.Length, "PlayerStates array does not have the correct length");

        for (int i=0; i < 20; i++)
        {
            Assert.AreEqual(1, states[i].Count, $"Incorrect number of states stored at frame {i}: {states[i].Count}");
            Assert.IsTrue(states[i].ContainsKey(0), $"Tail 0 does not have a state stored at frame {i}");
            
            PlayerState ps = states[i][0];
            Assert.AreEqual(0, ps.PlayerID, $"Player ID not recorded correctly at frame {i}");
            Assert.AreEqual(0, ps.TailID, $"Tail ID not recorded correctly at frame {i}");
            Assert.AreEqual(new Vector3(i, i, i), ps.Pos, $"Position not recorded correctly at frame {i}");
            Assert.AreEqual(new Quaternion(i, i, i, i), ps.Rot, $"Rotation not recorded correctly at frame {i}");
            Assert.AreEqual(dir, ps.JumpDirection, $"Jump direction not recorded correctly at frame {i}");
            Assert.IsFalse(ps.Kill, $"Kill not recorded correcty at frame {i}");
        }

        Debug.Log("All assertions pass.");
    }
}
