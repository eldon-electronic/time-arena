using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestRealityManager
{   
    [Test]
    public void TestAddHead()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);

        Dictionary<int, Reality> heads = manager.RevealHeads();
        Assert.AreEqual(1, heads.Count);
        Assert.IsTrue(heads.ContainsKey(1001));
        Assert.IsTrue(heads[1001] != null);
    }

    [Test]
    public void TestGetPerceivedFrame()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);
        int percievedframe = manager.GetPerceivedFrame(1001);
        Assert.AreEqual(0, percievedframe);
    }

    [Test]
    public void TestGetPerceivedFrames()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);
        manager.AddHead(1002);
        List<(int, int)> percievedFrames = manager.GetPerceivedFrames();
        
        Assert.AreEqual(2, percievedFrames.Count);
        Assert.IsTrue(percievedFrames.Contains((1001, 0)));
        Assert.IsTrue(percievedFrames.Contains((1002, 0)));
    }

    [Test]
    public void TestSetPerceivedFrame()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);
        manager.SetPerceivedFrame(1001, 50);

        Dictionary<int, Reality> heads = manager.RevealHeads();
        Assert.AreEqual(50, heads[1001].PerceivedFrame);
    }

    [Test]
    public void TestOffsetPerceivedFrame()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);

        manager.OffsetPerceivedFrame(1001, 50);
        Dictionary<int, Reality> heads = manager.RevealHeads();
        Assert.AreEqual(50, heads[1001].PerceivedFrame);

        manager.OffsetPerceivedFrame(1001, -10);
        heads = manager.RevealHeads();
        Assert.AreEqual(40, heads[1001].PerceivedFrame);
    }

    [Test]
    public void TestAddWriter()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);

        manager.AddWriter(1001, 50);
        Dictionary<int, Reality> heads = manager.RevealHeads();
        Assert.AreEqual(1, heads[1001].WriteFrames.Count);
        Assert.AreEqual(50, heads[1001].WriteFrames[0]);

        manager.AddWriter(1001, 100);
        heads = manager.RevealHeads();
        Assert.AreEqual(2, heads[1001].WriteFrames.Count);
        Assert.AreEqual(50, heads[1001].WriteFrames[0]);
        Assert.AreEqual(100, heads[1001].WriteFrames[1]);

        try
        {
            manager.AddWriter(1001, 150);
            Assert.IsTrue(false);
        }
        catch (InvalidOperationException e)
        {
            Assert.IsTrue(true);
        }
    }

    [Test]
    public void TestGetWriteFrames()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);

        List<int> frames = manager.GetWriteFrames(1001);
        Assert.AreEqual(0, frames.Count);

        manager.AddWriter(1001, 50);
        frames = manager.GetWriteFrames(1001);
        Assert.AreEqual(1, frames.Count);
        Assert.AreEqual(50, frames[0]);

        manager.AddWriter(1001, 100);
        frames = manager.GetWriteFrames(1001);
        Assert.AreEqual(2, frames.Count);
        Assert.IsTrue(frames.Contains(50));
        Assert.IsTrue(frames.Contains(100));
    }

    [Test]
    public void TestRemoveWriter()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);

        try
        {
            manager.RemoveWriter(1001);
            Assert.IsTrue(false);
        }
        catch (InvalidOperationException e)
        {
            Assert.IsTrue(true);
        }

        manager.AddWriter(1001, 50);
        manager.AddWriter(1001, 100);

        manager.RemoveWriter(1001);
        Dictionary<int, Reality> heads = manager.RevealHeads();
        Assert.AreEqual(1, heads[1001].WriteFrames.Count);
        Assert.AreEqual(100, heads[1001].WriteFrames[0]);

        manager.RemoveWriter(1001);
        heads = manager.RevealHeads();
        Assert.AreEqual(0, heads[1001].WriteFrames.Count);
    }

    [Test]
    public void TestGetClosestFrame()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);
        manager.SetPerceivedFrame(1001, 50);

        // No other players => returns max int value.
        int frame = manager.GetClosestFrame(1001);
        Assert.AreEqual(int.MaxValue, frame);

        // One other player => returns the value of their last writer.
        manager.AddHead(1002);
        manager.AddWriter(1002, 49);
        manager.AddWriter(1002, 10);

        frame = manager.GetClosestFrame(1001);
        Assert.AreEqual(10, frame);

        // Multiple other players => returns the closest.
        manager.AddHead(1003);
        manager.AddWriter(1003, 60);

        frame = manager.GetClosestFrame(1001);
        Assert.AreEqual(60, frame);
    }

    [Test]
    public void TestGetHeadsInFrame()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);

        // No writer => in the void, not in a reality => returns nothing.
        HashSet<int> heads = manager.GetHeadsInFrame(0);
        Assert.AreEqual(0, heads.Count);

        // One writer at frame 50.
        manager.AddWriter(1001, 50);
        heads = manager.GetHeadsInFrame(50);
        Assert.AreEqual(1, heads.Count);
        Assert.IsTrue(heads.Contains(1001));

        heads = manager.GetHeadsInFrame(0);
        Assert.AreEqual(0, heads.Count);

        // One player with multiple writers at frame 50.
        manager.AddWriter(1001, 50);
        heads = manager.GetHeadsInFrame(50);
        Assert.AreEqual(1, heads.Count);
        Assert.IsTrue(heads.Contains(1001));

        // Multiple players with writers at frame 50.
        manager.AddHead(1002);
        manager.AddWriter(1002, 50);
        heads = manager.GetHeadsInFrame(50);
        Assert.AreEqual(2, heads.Count);
        Assert.IsTrue(heads.Contains(1001));
        Assert.IsTrue(heads.Contains(1002));
    }

    [Test]
    public void TestGetAllHeads()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);

        // One player.
        List<int> heads = manager.GetAllHeads(1001);
        Assert.AreEqual(0, heads.Count);

        // Multiple players.
        manager.AddHead(1002);
        manager.AddHead(1003);
        heads = manager.GetAllHeads(1001);
        Assert.AreEqual(2, heads.Count);
        Assert.IsTrue(heads.Contains(1002));
        Assert.IsTrue(heads.Contains(1003));
    }

    [Test]
    public void TestInSameFrame()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);
        manager.AddHead(1002);

        // Same frame.
        Assert.IsTrue(manager.InSameFrame(1001, 1002));

        // Different frame.
        manager.SetPerceivedFrame(1002, 50);
        Assert.IsFalse(manager.InSameFrame(1001, 1002));
    }

    [Test]
    public void TestGetLastTailID()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);

        int tailID = manager.GetLastTailID(1001);
        Assert.AreEqual(100100, tailID);
    }

    [Test]
    public void TestGetNextTailID()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);

        // No write frames.
        int tailID = manager.GetNextTailID(1001);
        Assert.AreEqual(100100, tailID);

        // One write frame.
        manager.AddWriter(1001, 50);
        tailID = manager.GetNextTailID(1001);
        Assert.AreEqual(100100, tailID);

        // Two write frames.
        manager.AddWriter(1001, 100);
        tailID = manager.GetNextTailID(1001);
        Assert.AreEqual(100101, tailID);
    }

    [Test]
    public void TestTick()
    {
        RealityManager manager = new RealityManager();
        manager.AddHead(1001);
        manager.AddHead(1002);
        manager.Tick();

        // Perceived frames are incremented.
        Dictionary<int, Reality> heads = manager.RevealHeads();
        Assert.AreEqual(1, heads[1001].PerceivedFrame);
        Assert.AreEqual(1, heads[1002].PerceivedFrame);

        // Countdown reaches 0.
        manager.AddWriter(1001, 50);
        manager.RemoveWriter(1001);

        for (int i=0; i < Constants.AnimationFrames; i++)
        {
            manager.Tick();
        }

        heads = manager.RevealHeads();
        Assert.AreEqual(1 + Constants.AnimationFrames, heads[1001].PerceivedFrame);
        Assert.AreEqual(1 + Constants.AnimationFrames, heads[1002].PerceivedFrame);
        Assert.AreEqual(0, heads[1001].WriteFrames.Count);
        Assert.AreEqual(100101, heads[1001].LastTailID);
    }
}
