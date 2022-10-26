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

    private bool CountNextTouch;
    private bool BlockedUntilNext;

    private void Start()
    {
        IsMobile = SystemInfo.operatingSystem.Contains("Android");

        BlockedUntilNext = false;
    }

    private void Update()
    {
        if (LaneController.HasPlayerCollided)
            return;

        if (IsMobile)
            Mobile_CarControl();
        else
            PC_CarControl();
    }

    /// <summary>
    /// Warning! The control scheme is unpolished. F2 the method when you finish making it as user friendly as possible
    /// </summary>
    private void Mobile_CarControl()
    {
        //if (CarController.GetPlayerCar() == null || CarController.GetPlayerCar().IsTurningNow)
        //    return;

        //------------
        //The default input system
        //1. Without a built in time limiter, falls apart. In general, prone to breaking even with it
        //2. That's it

        //var touch = Input.GetTouch(0);
        //if (touch.phase != TouchPhase.Moved)
        //    return;

        //if (touch.deltaPosition.x < 0f)
        //    CarController.GetPlayerCar().StartChangingLane(toLeft: true);
        //else if (touch.deltaPosition.x > 0f)
        //    CarController.GetPlayerCar().StartChangingLane(toLeft: false);

        //---------------
        //Not a very bad input system, it's very stable, but:
        //1. has the obvious input delay because it's based on TouchPhase.Ended
        //2. needs a time out: if between phase.Began and phase.Ended is more than 1-2s, abort the touch

        //var touch = Input.GetTouch(0);

        //if(Input.touchCount > 0 && touch.phase == TouchPhase.Began)
        //{
        //    startPos = touch.position;
        //}

        //if(Input.touchCount > 0 && touch.phase == TouchPhase.Ended)
        //{
        //    endPos = touch.position;

        //    if (startPos.x > endPos.x)
        //        CarController.GetPlayerCar().StartChangingLane(toLeft: true);
        //    if (startPos.x < endPos.x)
        //        CarController.GetPlayerCar().StartChangingLane(toLeft: false);
        //}

        //---------------
        //Input?
        //1. It's fast
        //2. But doesn't work with gentle touches... If you swipe furiously, that'll work very fast!
        //   Probably because doesn't immediately go Began -> Moved, but Began -> Stationary -> Moved
        //3. Could be good to instead block touch until it reached touch end

        //var touch = Input.GetTouch(0);

        //if (touch.phase == TouchPhase.Began)
        //{
        //    CountNextTouch = true;
        //    return;
        //}

        //if(touch.phase == TouchPhase.Moved && CountNextTouch)
        //{
        //    CountNextTouch = false;

        //    if (touch.deltaPosition.x < 0f)
        //        CarController.GetPlayerCar().StartChangingLane(toLeft: true);
        //    else if (touch.deltaPosition.x > 0f)
        //        CarController.GetPlayerCar().StartChangingLane(toLeft: false);
        //}
        //else
        //{
        //    CountNextTouch = false;
        //}

        //-------------------
        //The perfect input system
        //1. It's fast
        //2. It's reliable
        //3. What else do you need?

        //Savings against IndexOutOfBound spam on Android
        if ( !(Input.touchCount > 0) )
            return;

        //The _return_ above ^^^ shouldn't do anything with the logic below, because in case of
        //the related exception Unity just refused to update script below Input.GetTouch(0);
        var touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Ended)
        {
            BlockedUntilNext = false;
        }

        if (touch.phase == TouchPhase.Moved && !BlockedUntilNext)
        {
            if (touch.deltaPosition.x < 0f)
            {
                CarController.GetPlayerCar().StartChangingLane(toLeft: true);
                BlockedUntilNext = true;
            }
            else if (touch.deltaPosition.x > 0f)
            {
                CarController.GetPlayerCar().StartChangingLane(toLeft: false);
                BlockedUntilNext = true;
            }
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
