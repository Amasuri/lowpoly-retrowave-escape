using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerRun : MonoBehaviour
{
    public const int screenWidthReference = 1080;
    public const int screenHeightReference = 2340;

    public Camera camera;
    static public CameraControllerRun current;

    private readonly Vector3 startRotVec = new Vector3(27, 90, 0);
    private readonly Vector3 endRotVec = new Vector3(15, 90, 0);

    private readonly Vector3 startPosOffset = new Vector3(-5, 4, 0); //PC resolution is fine with y=4, but it looks ugh on mobile, so
    private readonly Vector3 endPosOffset = new Vector3(-7, 4, 0);
    private Vector3 currentPosOffset;

    private const float startFoV = 60;
    private const float endFoV = 90;

    // Start is called before the first frame update
    private void Start()
    {
        //Screen.SetResolution(screenWidth, screenHeight, true); //---> Devices might downscale automatically. In case of problems, force resolution
        current = this;
        ResetSpeedEffects();
    }

    // Update is called once per frame
    private void Update()
    {
        if (LaneController.HasPlayerCollided)
            return;

        if (LaneController.current.HasPlayerBeenSpawned)
            ChasePlayerCar();
        else
            ChaseConstantPace();

        CorrectAngleBasedOnElapsedTime();
        CorrectFoVBasedOnElapsedTime();
        CorrectPosOffsetBasedOnElapsedTime();
    }

    public void ResetSpeedEffects()
    {
        currentPosOffset = startPosOffset;
        transform.rotation = Quaternion.Euler(startRotVec);
        camera.fieldOfView = startFoV;
    }

    private void ChasePlayerCar()
    {
        var pCar = CarController.GetPlayerCar();

        if (pCar != null)
            transform.position = pCar.transform.position + currentPosOffset;
    }

    private void ChaseConstantPace()
    {
        var movVec = new Vector3(Time.deltaTime * CarController.speedFactorCurrent, 0, 0);
        transform.position += movVec;
    }

    private void CorrectAngleBasedOnElapsedTime()
    {
        var rotLength = startRotVec.x - endRotVec.x; //wanted to make it a constant, but guess what? fields not supported

        //Remaps elapsed time from time range 0-60s to camera degree range 0-12 degrees delta
        var currentCameraRot = MathHelper.Remap(RunTimer.TimeSinceLastRunStartSec, 0f, 60f, 0f, rotLength);
        var rotVec = startRotVec - new Vector3(currentCameraRot, 0, 0);
        if (currentCameraRot >= rotLength)
            rotVec = endRotVec;

        transform.rotation = Quaternion.Euler(rotVec);
    }

    private void CorrectFoVBasedOnElapsedTime()
    {
        var maxFoVdelta = endFoV - startFoV;

        //Remaps elapsed time from time range 0-60s to camera FoV range 0-15 degrees delta
        var currentFoVdelta = MathHelper.Remap(RunTimer.TimeSinceLastRunStartSec, 0f, 60f, 0f, maxFoVdelta);
        var resultFoV = startFoV + currentFoVdelta;
        if (currentFoVdelta >= maxFoVdelta)
            resultFoV = endFoV;

        camera.fieldOfView = resultFoV;
    }

    private void CorrectPosOffsetBasedOnElapsedTime()
    {
        var maxPosXdelta = startPosOffset.x - endPosOffset.x;

        //Remaps elapsed time from time range 0-60s to camera position offset 0-1 by X coordinate (Y not counted)
        var currentPosXdelta = MathHelper.Remap(RunTimer.TimeSinceLastRunStartSec, 0f, 60f, 0f, maxPosXdelta);
        var resultPosX = startPosOffset.x - currentPosXdelta;
        if (currentPosXdelta >= maxPosXdelta)
            resultPosX = endPosOffset.x;

        currentPosOffset = new Vector3(resultPosX, startPosOffset.y, startPosOffset.z);
    }
}
