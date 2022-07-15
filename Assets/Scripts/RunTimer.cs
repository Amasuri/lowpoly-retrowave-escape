using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTimer : MonoBehaviour
{
    static public float TimeSinceLastRunStartSec { get; private set; }
    static private float startTimeMs;
    static public bool IsCounting { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        TimeSinceLastRunStartSec = 0f;
        startTimeMs = 0f;
        IsCounting = false;

        StartNewCount();
    }

    // Update is called once per frame
    private void Update()
    {
        if(IsCounting)
            TimeSinceLastRunStartSec = Time.realtimeSinceStartup - startTimeMs;
    }

    static public void StartNewCount()
    {
        startTimeMs = Time.realtimeSinceStartup;
        IsCounting = true;
    }
}
