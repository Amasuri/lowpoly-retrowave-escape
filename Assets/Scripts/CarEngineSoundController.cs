using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngineSoundController : MonoBehaviour
{
    public AudioSource soundPlayer;

    public bool isPlayerCar;

    private const float startPitch = 0.75f; //0.6-0.7 works
    private const float endPitch = 2f;

    // Start is called before the first frame update
    private void Start()
    {
        soundPlayer.pitch = startPitch;
    }

    // Update is called once per frame
    private void Update()
    {
        if(LaneController.HasPlayerCollided)
        {
            soundPlayer.enabled = false;
            return;
        }

        var maxPitchDelta = endPitch - startPitch;

        //Remaps elapsed time from time range 0-60s to camera FoV range 0-15 degrees delta
        var currentPitchDelta = MathHelper.Remap(RunTimer.TimeSinceLastRunStartSec, 0f, 60f, 0f, maxPitchDelta);
        var resultPitch = startPitch + currentPitchDelta;
        if (currentPitchDelta >= maxPitchDelta)
            resultPitch = endPitch;

        soundPlayer.pitch = resultPitch;
    }
}
