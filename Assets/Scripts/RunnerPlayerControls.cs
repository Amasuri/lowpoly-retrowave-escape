using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class RunnerPlayerControls : MonoBehaviour
{
    public bool IsMobile { get; private set; }

    private Vector2 startPos;
    private Vector2 endPos;

    // Start is called before the first frame update
    private void Start()
    {
        IsMobile = SystemInfo.operatingSystem.Contains("Android");
    }

    // Update is called once per frame
    private void Update()
    {
        if (LaneController.HasPlayerCollided)
            return;

        if (IsMobile)
            DebugMobile_CarControl();
        else
            PC_CarControl();
    }

    /// <summary>
    /// Warning! The control scheme is unpolished. F2 the method when you finish making it as user friendly as possible
    /// </summary>
    private void DebugMobile_CarControl()
    {
        //if (CarController.GetPlayerCar() == null || CarController.GetPlayerCar().IsTurningNow)
        //    return;

        //var touch = Touchscreen.current.primaryTouch;

        //var touch = Input.GetTouch(0);
        //if (touch.phase != TouchPhase.Moved)
        //    return;

        //if (touch.deltaPosition.x < 0f)
        //    CarController.GetPlayerCar().StartChangingLane(toLeft: true);
        //else if (touch.deltaPosition.x > 0f)
        //    CarController.GetPlayerCar().StartChangingLane(toLeft: false);

        //---------------
        //Not a very bad input system, but:
        //1. has the obvious input delay because it's based on TouchPhase.Ended
        //2. needs a time out: if between phase.Began and phase.Ended is more than 1-2s, abort the touch

        var touch = Input.GetTouch(0);

        if(Input.touchCount > 0 && touch.phase == TouchPhase.Began)
        {
            startPos = touch.position;
        }

        if(Input.touchCount > 0 && touch.phase == TouchPhase.Ended)
        {
            endPos = touch.position;

            if (startPos.x > endPos.x)
                CarController.GetPlayerCar().StartChangingLane(toLeft: true);
            if (startPos.x < endPos.x)
                CarController.GetPlayerCar().StartChangingLane(toLeft: false);
        }
    }

    /// <summary>
    /// Warning! For PC debug only. You should implement different control scheme when you make a mobile build.
    /// </summary>
    private void PC_CarControl()
    {
        //if (CarController.GetPlayerCar() == null || CarController.GetPlayerCar().IsTurningNow)
        //    return;

        if (Input.GetKeyDown(KeyCode.A))
            CarController.GetPlayerCar().StartChangingLane(toLeft: true);
        else if (Input.GetKeyDown(KeyCode.D))
            CarController.GetPlayerCar().StartChangingLane(toLeft: false);
    }
}
