using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Essentially, what it does is that if Close Call happened and player failed immediately afterwards, then it cancels the score bonus
/// from Close Call
/// </summary>
public class CloseCallFailureEvent : MonoBehaviour
{
    public static event CloseCallFail OnCloseCallFail;

    public delegate void CloseCallFail();

    private void Start()
    {
        PlayerCrashEvent.OnPlayerCrash += CheckFailedCC_OnCollision;
    }

    private void CheckFailedCC_OnCollision()
    {
        //Current condition for failed CC: do anything that makes you collide in "grace" period
        //The reasoning is: no matter what you did, your action was immediate after the CC
        //So it's seen in context of poor future planning
        //(Also it's just easier to understand, rather than having multiple flanking rules and remembering CC colliders and such)
        if (RunGUITimeScoreText.current.HadCloseCallRecently)
            OnCloseCallFail();
    }

    private void OnDestroy()
    {
        PlayerCrashEvent.OnPlayerCrash -= CheckFailedCC_OnCollision;
    }

    private void OnDisable()
    {
        PlayerCrashEvent.OnPlayerCrash -= CheckFailedCC_OnCollision;
    }
}
