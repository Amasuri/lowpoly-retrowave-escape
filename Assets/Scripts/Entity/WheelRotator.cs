using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    public bool isPlayerCar;
    public bool isPlane;

    // Update is called once per frame
    private void Update()
    {
        if (!isPlane)
        {
            if (transform.parent.GetComponent<CarController>().HasThisCarCollided)
                return;
        }

        var rotDelta = new Vector3(Time.deltaTime * 1500, 0, 0);
        if (isPlayerCar)
            rotDelta = new Vector3(0, 0, Time.deltaTime * 1500);

        transform.Rotate(rotDelta);
    }
}
