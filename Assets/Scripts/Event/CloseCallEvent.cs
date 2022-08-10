using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCallEvent : MonoBehaviour
{
    public BoxCollider CloseCallCollider;

    public static event CloseCall OnCloseCall;

    public delegate void CloseCall();

    private void OnTriggerEnter(Collider other)
    {
        //if it's a car,car is player car and player hasn't collided, then begin event
        if (other.gameObject.GetComponent<CarController>() == null)
            return;
        if (!other.gameObject.GetComponent<CarController>().IsPlayerCar)
            return;
        if (LaneController.HasPlayerCollided)
            return;

        OnCloseCall();
    }
}
