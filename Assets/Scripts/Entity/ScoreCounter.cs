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

    private void Start()
    {
        current = this;
        Reset();
    }

    /// <summary>
    /// DON'T use this for score calculations! Ask the same class for current score.
    /// This is used mainly for debug.
    /// </summary>
    public int GetCloseCallTimes()
    {
        return CloseCallTimes;
    }

    public void Reset()
    {
        CloseCallTimes = 0;
    }

    private void IncreaseCloseCallTimes()
    {
        CloseCallTimes++;
    }
}
