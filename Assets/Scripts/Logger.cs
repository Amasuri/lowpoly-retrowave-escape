using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wraps around Debug.Log for more variable logging
/// </summary>
static public class Logger
{
    static public void Log(string str)
    {
        if (!SystemSettings.DEBUGLOG_ON)
            return;

        Debug.Log(str);
    }
}
