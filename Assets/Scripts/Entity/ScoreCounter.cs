using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter current;

    public int TotalScore => (int)TimeScore + (int)CloseCallScore;
    private float TimeScore => RunTimer.TimeSinceLastRunStartSec * 100f;
    private float CloseCallScore => CloseCallTimes * 500f;
    private int CloseCallTimes;

    public float CloseCallScoreCurrentOnetimeBonus => MathHelper.RemapAndLimitToRange(RunTimer.TimeSinceLastRunStartSec, 0f, 60f, CloseCallScoreMin, CloseCallScoreMax);
    private const float CloseCallScoreMin = CarController.startSpeed * 20; //curr 300f
    private const float CloseCallScoreMax = (CarController.speedLimit * 20) + 300; //curr 900+300 = 1200f

    private float LastCloseCallDebug;

    private void Start()
    {
        current = this;
        Reset();
    }

    /// <summary>
    /// DON'T use this for score calculations! Ask the same class for current score.
    /// This is used mainly for debug.
    /// </summary>
    public int GetCloseCallTimesDebug()
    {
        return CloseCallTimes;
    }

    /// <summary>
    /// DON'T use this for score calculations! Ask the same class for current score.
    /// This is used mainly for debug.
    /// </summary>
    public float GetLastCloseCallDebug()
    {
        return LastCloseCallDebug;
    }

    public void Reset()
    {
        CloseCallTimes = 0;
        CloseCallEvent.OnCloseCall += IncreaseCloseCallTimes;
        LastCloseCallDebug = 0f;
    }

    private void IncreaseCloseCallTimes()
    {
        CloseCallTimes++;
        LastCloseCallDebug = CloseCallScoreCurrentOnetimeBonus;
    }
}
