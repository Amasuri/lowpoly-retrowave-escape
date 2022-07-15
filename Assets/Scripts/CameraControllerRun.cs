using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerRun : MonoBehaviour
{
    private readonly Vector3 startRotVec = new Vector3(27, 90, 0);
    private readonly Vector3 endRotVec = new Vector3(15, 90, 0);

    private readonly Vector3 defaultOffset = new Vector3(-7, 4, 0);

    // Start is called before the first frame update
    private void Start()
    {
        transform.rotation = Quaternion.Euler(startRotVec);
    }

    // Update is called once per frame
    private void Update()
    {
        if (LaneController.HasPlayerCollided)
            return;

        ChasePlayerCar();
        CorrectAngleBasedOnElapsedTime();
    }

    private void ChasePlayerCar()
    {
        var pCar = CarController.GetPlayerCar();

        if (pCar != null)
            transform.position = pCar.transform.position + defaultOffset;
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
}
