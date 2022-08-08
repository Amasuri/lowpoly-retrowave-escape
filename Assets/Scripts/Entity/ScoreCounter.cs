using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter current;

    public int TotalScore => (int)TimeScore + (int)CloseCallScore;
    private float TimeScore => RunTimer.TimeSinceLastRunStartSec * 100f;
    private float CloseCallScore => CloseCallList.Sum();

    public float CloseCallScoreCurrentOnetimeBonus => MathHelper.RemapAndLimitToRange(RunTimer.TimeSinceLastRunStartSec, 0f, 60f, CloseCallScoreMin, CloseCallScoreMax);
    private const float CloseCallScoreMin = CarController.startSpeed * 20; //curr 300f
    private const float CloseCallScoreMax = (CarController.speedLimit * 20) + 300; //curr 900+300 = 1200f
    private readonly List<float> CloseCallList = new List<float>();

    private float LastCloseCallDebug;
    private int CloseCallTimesDebug;

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
        return CloseCallTimesDebug;
    }

    /// <summary>
    /// DON'T use this for score calculations! Ask the same class for current score.
    /// This is used mainly for debug.
    /// </summary>
    public float GetLastCloseCallDebug()
    {
        return LastCloseCallDebug;
    }

    /// <summary>
    /// DON'T use this for score calculations! Ask the same class for current score.
    /// This is used mainly for debug.
    /// </summary>
    public float GetAllCloseCallDebug()
    {
        return CloseCallScore;
    }

    public void Reset()
    {
        CloseCallEvent.OnCloseCall += RegisterNewCloseCall;

        CloseCallTimesDebug = 0;
        LastCloseCallDebug = 0f;
        CloseCallList.Clear();
    }

    private void RegisterNewCloseCall()
    {
        CloseCallTimesDebug++;
        LastCloseCallDebug = CloseCallScoreCurrentOnetimeBonus;
        CloseCallList.Add(CloseCallScoreCurrentOnetimeBonus);
    }
}
