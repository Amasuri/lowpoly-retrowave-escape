using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static variables described here refer to the player. Since no one aside from the player
/// is able to shift gears & it won't change; so exhaust control is purely player
/// </summary>
public class CarExhaustSoundController : MonoBehaviour
{
    public static int TimesShifted { get; private set; }
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

    private void Start()
    {
        TimesShifted = 0;
        LastShiftWasAt = 0f;
        NextShiftWillBeAt = firstShiftAtSec;
    }

    private void Update()
    {
        if (RunTimer.TimeSinceLastRunStartSec >= firstShiftAtSec && TimesShifted == 0)
            PlayExhaust();
        else if (RunTimer.TimeSinceLastRunStartSec >= secondShiftAtSec && TimesShifted == 1)
            PlayExhaust();
        else if (RunTimer.TimeSinceLastRunStartSec >= thirdShiftAtSec && TimesShifted == 2)
            PlayExhaust();
        else if (RunTimer.TimeSinceLastRunStartSec >= fourthShiftAtSec && TimesShifted == 3)
            PlayExhaust();
    }

    private void PlayExhaust()
    {
        LastShiftWasAt = AmountShiftedToTime[TimesShifted];
        NextShiftWillBeAt = AmountShiftedToTime[TimesShifted+1];

        TimesShifted++;
        soundPlayer.Play();

        OnEngineExhaust();
    }
}
