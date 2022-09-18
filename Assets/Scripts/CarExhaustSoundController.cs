using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarExhaustSoundController : MonoBehaviour
{
    public static float LastShiftWasAt { get; private set; }
    public static float NextShiftWillBeAt { get; private set; }
    private const float firstShiftAtSec = 8f;
    private const float secondShiftAtSec = 18f;
    private const float thirdShiftAtSec = 35f;
    private const float fourthShiftAtSec = 60f;

    private Dictionary<int, float> AmountShiftedToTime = new Dictionary<int, float>
    {
        { 0, firstShiftAtSec },
        { 1, secondShiftAtSec },
        { 2, thirdShiftAtSec },
        { 3, fourthShiftAtSec },
        { 4, fourthShiftAtSec * 1.5f },
    };

    public AudioSource soundPlayer;

    public static event EngineExhaust OnEngineExhaust;

    public delegate void EngineExhaust();

    private int timesShifted = 0;

    private void Start()
    {
        timesShifted = 0;
        LastShiftWasAt = 0f;
        NextShiftWillBeAt = firstShiftAtSec;
    }

    private void Update()
    {
        if (RunTimer.TimeSinceLastRunStartSec >= firstShiftAtSec && timesShifted == 0)
            PlayExhaust();
        else if (RunTimer.TimeSinceLastRunStartSec >= secondShiftAtSec && timesShifted == 1)
            PlayExhaust();
        else if (RunTimer.TimeSinceLastRunStartSec >= thirdShiftAtSec && timesShifted == 2)
            PlayExhaust();
        else if (RunTimer.TimeSinceLastRunStartSec >= fourthShiftAtSec && timesShifted == 3)
            PlayExhaust();
    }

    private void PlayExhaust()
    {
        LastShiftWasAt = AmountShiftedToTime[timesShifted];
        NextShiftWillBeAt = AmountShiftedToTime[timesShifted+1];

        timesShifted++;
        soundPlayer.Play();

        OnEngineExhaust();
    }
}
