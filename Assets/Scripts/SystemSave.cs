using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SystemSave : MonoBehaviour
{
    static public int CumulativeScore { get; private set; }
    static public int HighestScore { get; private set; }

    static private bool FirstActivationInSession = true;

    private string path => Application.persistentDataPath + Path.DirectorySeparatorChar + file;
    private const string file = "save.json";

    private void Start()
    {
        PlayerCrashEvent.OnPlayerCrash += RecordNewValues;

        if(FirstActivationInSession)
        {
            //Check for file and read; else create. Exception intends to catch badly formatted files (those shouldn't happen!)
            if (File.Exists(path))
            {
                try
                {
                    var str = File.ReadAllLines(path);
                    CumulativeScore = Convert.ToInt32(str[0]);
                    HighestScore = Convert.ToInt32(str[1]);
                }

                //There's a possibility, if some system decides to have different newline strings like /n /r and such, and save files were moved (crazy!) between systems
                //That, or file got corrupted (somehow).
                //Supported: Windows, Android. If I get to HTML5 dev, there probably will be a I/O problem here
                catch (Exception)
                {
                    CumulativeScore = 0;
                    HighestScore = 0;
                }
            }

            //If file doesn't exist...
            else
            {
                File.Create(path);
                CumulativeScore = 0;
                HighestScore = 0;
            }

            FirstActivationInSession = false;
        }
    }

    private void RecordNewValues()
    {
        var score = ScoreCounter.current.TotalScore;

        CumulativeScore += score;

        if (score > HighestScore)
            HighestScore = score;

        //File creation check should happen at game start
        File.WriteAllLines(path, new string[] { CumulativeScore.ToString(), HighestScore.ToString() });
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
