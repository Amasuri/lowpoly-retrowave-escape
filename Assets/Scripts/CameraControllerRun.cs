using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerRun : MonoBehaviour
{
    private readonly Quaternion defaultRotation = new Quaternion(0.2f, 0.7f, -0.2f, 0.7f); //~= degrees 27 90 0
    private readonly Vector3 defaultOffset = new Vector3(-7, 4, 0);

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log(transform.position);

        transform.rotation = defaultRotation;
    }

    // Update is called once per frame
    private void Update()
    {
        ChasePlayerCar();
    }

    private void ChasePlayerCar()
    {
        var pCar = CarController.GetPlayerCar();

        if (pCar != null)
            transform.position = pCar.transform.position + defaultOffset;
    }
}
