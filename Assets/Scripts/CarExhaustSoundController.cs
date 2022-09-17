using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarExhaustSoundController : MonoBehaviour
{
    public AudioSource soundPlayer;

    public static event EngineExhaust OnEngineExhaust;

    public delegate void EngineExhaust();

    private int timesShifted = 0;

    private void Start()
    {
        timesShifted = 0;
    }

    private void Update()
    {
        if (RunTimer.TimeSinceLastRunStartSec >= 8f && timesShifted == 0)
            PlayExhaust();
        else if (RunTimer.TimeSinceLastRunStartSec >= 18f && timesShifted == 1)
            PlayExhaust();
        else if (RunTimer.TimeSinceLastRunStartSec >= 35f && timesShifted == 2)
            PlayExhaust();
        else if (RunTimer.TimeSinceLastRunStartSec >= 60f && timesShifted == 3)
            PlayExhaust();
    }

    private void PlayExhaust()
    {
        timesShifted++;
        soundPlayer.Play();

        OnEngineExhaust();
    }
}
