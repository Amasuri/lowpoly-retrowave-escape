using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Counts all crashes across all scenes and games. Hence a foreign static entity
/// </summary>
public static class PlayerCrashEventCounter
{
    //Counts all crashes across all scenes and games. Hence a foreign static entity
    static public int CrashTimes { get; private set; }

    //Static constructor doesn't reset or get called on scene start (because no scene tied to it)
    static PlayerCrashEventCounter()
    {
        CrashTimes = 0;
        PlayerCrashEvent.OnPlayerCrash += IncreaseCrashTimes;
    }

    static private void IncreaseCrashTimes()
    {
        CrashTimes++;
    }

    //Doesn't need an event destructor. Because it's lifespan == app lifespan
}
