using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemSave : MonoBehaviour
{
    static public float CumulativeScore { get; private set; }
    static public float HighestScore { get; private set; }

    static private bool FirstActivationInSession = true;

    private void Start()
    {
        PlayerCrashEvent.OnPlayerCrash += RecordNewValues;

        if(FirstActivationInSession)
        {
            //todo stuff like reading serialized save states
            ;

            CumulativeScore = 0f;
            HighestScore = 0f;
            FirstActivationInSession = false;
        }
    }

    private void RecordNewValues()
    {
        var score = ScoreCounter.current.TotalScore;

        CumulativeScore += score;

        if (score > HighestScore)
            HighestScore = score;

        //todo stuff like saving serialized save states
        ;
    }

    private void OnDestroy()
    {
        PlayerCrashEvent.OnPlayerCrash -= RecordNewValues;
    }

    private void OnDisable()
    {
        PlayerCrashEvent.OnPlayerCrash -= RecordNewValues;
    }
}
