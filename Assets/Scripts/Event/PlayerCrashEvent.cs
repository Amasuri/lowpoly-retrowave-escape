using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrashEvent : MonoBehaviour
{
    public static event PlayerCrash OnPlayerCrash;

    public delegate void PlayerCrash();

    static public void CallPlayerCrashEvent()
    {
        //Order of call is such that it first does things on crash, and then records the crash
        if(!LaneController.HasPlayerCollided)
            OnPlayerCrash();
    }
}
