using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCallEvent : MonoBehaviour
{
    public BoxCollider CloseCallCollider;

    public static event CloseCall OnCloseCall;

    public delegate void CloseCall();

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

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
