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

    // Start is called before the first frame update
    private void Start()
    {
        current = this;
        Reset();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void Reset()
    {
        CloseCallTimes = 0;
    }
}
