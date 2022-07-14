using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerPlayerControls : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        PC_CarControl();
    }

    /// <summary>
    /// Warning! For PC debug only. You should implement different control scheme when you make a mobile build.
    /// </summary>
    private void PC_CarControl()
    {
        if (Input.GetKeyDown(KeyCode.A))
            CarController.GetPlayerCar().MoveToLeft();
        else if (Input.GetKeyDown(KeyCode.D))
            CarController.GetPlayerCar().MoveToRight();
    }
}