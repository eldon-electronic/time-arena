using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestRealityManager
{
    [Test]
    public void RealityTests()
    {
        Reality reality = new Reality();
        reality.Increment();
        Assert.AreEqual(reality.PerceivedFrame, 1, "increment() not working");
    }
    
    [Test]
    public void RealityManagerTests()
    {
        RealityManager realityManager = new RealityManager();
        
        // Test that reality manager created correct structure. 
        Dictionary<int, Reality> _realities = realityManager.RevealHeads();
        Assert.AreEqual(0, _realities.Count, "_realities has not been created to the correct size");

        // Test adding players to the dictionary.
        realityManager.AddHead(0);
        realityManager.AddHead(1);
        _realities = realityManager.RevealHeads();
        Assert.IsTrue(_realities.ContainsKey(0), "_realities does not contain an entry for playerID 0");
        Assert.IsTrue(_realities.ContainsKey(1), "_realities does not contain an entry for playerID 1");
        Assert.AreEqual(_realities.Count, 2, "_realities is not the correct size");
        

        // Test the GetPercievedFrames and GetPercievedFrame methods
        int percievedframe = realityManager.GetPerceivedFrame(0);
        List<(int, int)> percievedFrames = realityManager.GetPerceivedFrames(); 
        Assert.AreEqual(0, percievedframe, "GetPercievedFrame not working");
        Assert.AreEqual(1, percievedFrames[1].Item1, "GetPercievedFrames not returning correct key");
        Assert.AreEqual(0, percievedFrames[1].Item2, "GetPercievedFrames not returning correct value");

        // Test realityManager Increment. 
        realityManager.Increment();
        percievedframe = realityManager.GetPerceivedFrame(1);
        Assert.AreEqual(1, percievedframe, "increment has not incremented the percieved frame by 1");

        // Test that OffsetPerceivedFrame adds the correct offset to the percieved frame.
        realityManager.OffsetPerceivedFrame(1, -20);
        realityManager.OffsetPerceivedFrame(0, 10);
        int percievedframe1 = realityManager.GetPerceivedFrame(1);
        int percievedframe0 = realityManager.GetPerceivedFrame(0);
        Assert.AreEqual(-19, percievedframe1, "offsetPercievedFrame has not worked with a negative offset");
        Assert.AreEqual(11, percievedframe0, "offsetPercievedFrame has not worked with a positive offset");

        // Test that the WriteFrames functions all work as intended.
        realityManager.AddWriter(0, 40);
        realityManager.AddWriter(0, 20);
        realityManager.AddWriter(1, 10000);
        realityManager.AddWriter(1, 6000);
        List<int> writeFrames0 = realityManager.GetWriteFrames(0);
        List<int> writeFrames1 = realityManager.GetWriteFrames(1);
        Assert.AreEqual(2, writeFrames0.Count, "AddWriter has not added correct amount of writers");
        Assert.AreEqual(10000, writeFrames1[0], "AddWriter has not added the corrent frame value");

        realityManager.RemoveWriter(0);
        Assert.AreEqual(20, realityManager.GetWriteFrames(0)[0], "RemoveWriter has removed the wrong writer.");
        Assert.AreEqual(1, realityManager.GetWriteFrames(0).Count, "RemoveWriter has removed wrong amount of writers");

        realityManager.Increment();
        Assert.AreEqual(6001, realityManager.GetWriteFrames(1)[1], "Increment has not incremented the value of writeframes");

        // Test that GetClosestFrames returns the closest player.
        realityManager.AddHead(2);
        realityManager.AddHead(3);
        realityManager.AddWriter(2, 17);
        realityManager.AddWriter(2, 10);
        realityManager.AddWriter(3, 11);
        Assert.AreEqual(10, realityManager.GetClosestFrame(3,12), "GetClosestFrame has not returned the closest frame");
        
        // Tests for GetLastTailID and GetNextTailID.
        Assert.AreEqual(0, realityManager.GetLastTailID(1), "GetLastTailID has not returned the correct value"); 
        Assert.AreEqual(1, realityManager.GetNextTailID(1), "GetNextTailID has not returned the correct value");

    }
}
