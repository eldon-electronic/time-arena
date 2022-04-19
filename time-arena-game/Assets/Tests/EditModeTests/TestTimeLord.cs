using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestTimeLord
{   
    [Test]
    public void TestConnect()
    {

    }

    [Test]
    public void TestGetTails()
    {
        TimeLord timeLord = new TimeLord(100);
        timeLord.Connect(1001, true);
        Dictionary<int, PlayerState> tails = timeLord.GetTailStates();
        Assert.AreEqual(0, tails.Count);
    }

    [Test]
    public void TestGetState()
    {

    }

    [Test]
    public void TestRecordState()
    {

    }

    [Test]
    public void TestTimeTravel()
    {

    }

    [Test]
    public void TestLeaveReality()
    {

    }

    [Test]
    public void TestGetNearestReality()
    {

    }

    [Test]
    public void TestEnterReality()
    {

    }

    [Test]
    public void TestSetPerceivedFrame()
    {

    }

    [Test]
    public void TestGetPlayerPositions()
    {

    }

    [Test]
    public void TestInYourReality()
    {

    }

    [Test]
    public void TestGetYourPosition()
    {

    }

    [Test]
    public void TestGetTimeProportion()
    {

    }

    [Test]
    public void TestGetElapsedTime()
    {

    }

    [Test]
    public void TestCanJump()
    {

    }

    [Test]
    public void TestGetPlayersInReality()
    {

    }

    [Test]
    public void TestGetAllPlayerIDs()
    {

    }
}
