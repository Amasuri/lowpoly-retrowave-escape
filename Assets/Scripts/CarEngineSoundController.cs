using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngineSoundController : MonoBehaviour
{
    public AudioSource soundPlayer;

    public bool isPlayerCar;

    private const float startPitch = 0.75f; //0.6-0.7 works
    private const float endPitch = 2f;

    private const float normalVolume = 0.5f;
    private const float exhaustVolume = 0.3f;
    private const float exhaustCDmaxSec = 1f;
    private float exhaustCDcurrentSec = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        if(isPlayerCar)
        {
            soundPlayer.pitch = startPitch;
            CarExhaustSoundController.OnEngineExhaust += StartExhaustEventTimer;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (isPlayerCar)
            ControlPlayerEngine();
        else
            ControlTrafficEngine();
    }

    private void ControlPlayerEngine()
    {
        if (LaneController.HasPlayerCollided)
        {
            soundPlayer.enabled = false;
            return;
        }

        if(exhaustCDcurrentSec > 0f)
        {
            exhaustCDcurrentSec -= Time.deltaTime;
            if (exhaustCDcurrentSec <= 0f)
                soundPlayer.volume = normalVolume;
        }

        var maxPitchDelta = endPitch - startPitch;

        //Remaps elapsed time from time range 0-60s to engine pitch range 0-1.25f delta
        var lastShift = CarExhaustSoundController.LastShiftWasAt;
        var nextShift = CarExhaustSoundController.NextShiftWillBeAt;
        var currentPitchDelta = MathHelper.Remap(RunTimer.TimeSinceLastRunStartSec, lastShift, nextShift, 0f, maxPitchDelta);
        var resultPitch = startPitch + currentPitchDelta;

        //After each shift there's slight increase in starting point
        resultPitch += 0.2f * CarExhaustSoundController.TimesShifted;

        //Sanity check
        if (currentPitchDelta >= maxPitchDelta)
            resultPitch = endPitch;

        //Applying + slight pitch drop right after the shift
        soundPlayer.pitch = resultPitch - (exhaustCDcurrentSec / 5);
    }

    private void ControlTrafficEngine()
    {
        if(transform.parent.gameObject.GetComponent<CarController>().HasThisCarCollided)
            soundPlayer.enabled = false;
    }

    private void StartExhaustEventTimer()
    {
        if (!isPlayerCar)
            return;

        soundPlayer.volume = exhaustVolume;
        exhaustCDcurrentSec = exhaustCDmaxSec;
    }

    private void OnDestroy()
    {
        CarExhaustSoundController.OnEngineExhaust -= StartExhaustEventTimer;
    }

    private void OnDisable()
    {
        CarExhaustSoundController.OnEngineExhaust -= StartExhaustEventTimer;
    }
}
