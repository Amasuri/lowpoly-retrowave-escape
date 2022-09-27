using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public bool IsAPlane;

    private void FixedUpdate()
    {
        if (!IsAPlane)
        {
            //Bird
            var movVec = new Vector3(
                Time.fixedDeltaTime * CarController.speedFactorCurrent / 2,
                0,
                -Time.fixedDeltaTime * CarController.speedFactorCurrent / 15); //10..20
            transform.position += movVec;
        }
        else
        {
            //Plane
            var movVec = new Vector3(
                Time.fixedDeltaTime * CarController.speedFactorCurrent * 1.5f,
                0,
                -Time.fixedDeltaTime * -1 * CarController.speedFactorCurrent / 10); //10..20
            transform.position += movVec;
        }
    }
}
