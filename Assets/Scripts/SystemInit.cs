using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemInit : MonoBehaviour
{
    private void Start()
    {
        //if you ever feel the need to have a Editor fps test, change to -1. Normal values are 270-330 fps
        Application.targetFrameRate = 60;
    }
}
