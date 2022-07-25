using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunnerPlayerControls : MonoBehaviour
{
    public bool IsMobile { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        IsMobile = SystemInfo.operatingSystem.Contains("Android");
    }

    // Update is called once per frame
    private void Update()
    {
        if(!IsMobile)
        {
            //Debug reload current scene
            if (Input.GetKeyDown(KeyCode.P))
                ResetAndReloadScene();
        }

        if (LaneController.HasPlayerCollided)
            return;

        if (IsMobile)
            DebugMobile_CarControl();
        else
            PC_CarControl();
    }

    public static void ResetAndReloadScene()
    {
        CarController.ResetAllCarsBeforeSceneWipe();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Warning! The control scheme is unpolished. F2 the method when you finish making it as user friendly as possible
    /// </summary>
    private void DebugMobile_CarControl()
    {
        if (CarController.GetPlayerCar() == null || CarController.GetPlayerCar().IsTurningNow)
            return;

        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Moved)
            return;

        if (touch.deltaPosition.x > 0f)
            CarController.GetPlayerCar().StartChangingLane(toLeft: true);
        else if (touch.deltaPosition.x < 0f)
            CarController.GetPlayerCar().StartChangingLane(toLeft: false);
    }

    /// <summary>
    /// Warning! For PC debug only. You should implement different control scheme when you make a mobile build.
    /// </summary>
    private void PC_CarControl()
    {
        if (CarController.GetPlayerCar() == null || CarController.GetPlayerCar().IsTurningNow)
            return;

        if (Input.GetKeyDown(KeyCode.A))
            CarController.GetPlayerCar().StartChangingLane(toLeft: true);
        else if (Input.GetKeyDown(KeyCode.D))
            CarController.GetPlayerCar().StartChangingLane(toLeft: false);
    }
}
