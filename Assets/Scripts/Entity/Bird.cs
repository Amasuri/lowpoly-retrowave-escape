using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private void FixedUpdate()
    {
        var movVec = new Vector3(
            Time.fixedDeltaTime * CarController.speedFactorCurrent / 2,
            0,
            -Time.fixedDeltaTime * CarController.speedFactorCurrent / 15); //10..20
        transform.position += movVec;
    }
}
