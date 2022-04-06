using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestReality
{
    [Test]
    public void TestInitialIncrement()
    {
        Reality reality = new Reality(1001);
        reality.Increment();
        Assert.AreEqual(1, reality.PerceivedFrame);
        Assert.AreEqual(0, reality.WriteFrames.Count);
        Assert.AreEqual(-1, reality.Countdown);
    }

    [Test]
    public void TestIncrementWithWriters()
    {
        Reality reality = new Reality(1001);
        reality.WriteFrames.Add(0);
        reality.WriteFrames.Add(1);
        reality.Increment();
        Assert.AreEqual(1, reality.PerceivedFrame);
        Assert.AreEqual(2, reality.WriteFrames.Count);
        Assert.AreEqual(1, reality.WriteFrames[0]);
        Assert.AreEqual(2, reality.WriteFrames[1]);
        Assert.AreEqual(-1, reality.Countdown);
    }

    [Test]
    public void TestIncrementWithCountdown()
    {
        Reality reality = new Reality(1001);
        reality.Countdown = 1;

        reality.Increment();
        Assert.AreEqual(0, reality.Countdown);

        reality.Increment();
        Assert.AreEqual(-1, reality.Countdown);

        reality.Increment();
        Assert.AreEqual(-1, reality.Countdown);
    }
}
