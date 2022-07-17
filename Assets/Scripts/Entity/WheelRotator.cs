using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (transform.parent.GetComponent<CarController>().HasThisCarCollided)
            return;

        var rotDelta = new Vector3(Time.deltaTime * 1500, 0, 0);

        transform.Rotate(rotDelta);
    }
}
