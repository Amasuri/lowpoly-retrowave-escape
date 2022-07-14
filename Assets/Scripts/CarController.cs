using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    static public List<CarController> allCars = new List<CarController>();

    public Collider collider;

    public bool IsPlayerCar;
    public bool HasThisCarCollided { get; private set; }

    private const float speedFactor = 15f;

    // Start is called before the first frame update
    private void Start()
    {
        allCars.Add(this);
        HasThisCarCollided = false;

        //Make sure passing cars get KO'd
        if (!IsPlayerCar)
            Destroy(gameObject, 15);
    }

    // Update is called once per frame
    private void Update()
    {
        if(!HasThisCarCollided)
            MoveForward();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Make sure you write only for car collisions
        if (collision.gameObject.GetComponent<CarController>() == null)
            return;

        HasThisCarCollided = true;

        if (IsPlayerCar)
            LaneController.RecordThatPlayerCollided();

        //Jerk the car a but upwards
        transform.Rotate(new Vector3(10, 0, 0));
    }

    static public CarController GetPlayerCar()
    {
        foreach (var car in allCars)
        {
            if (car.IsPlayerCar)
                return car;
        }

        return null;
    }

    public void ChangeLane(bool toLeft = true)
    {
        var dir = toLeft ? 1 : -1;

        transform.position += new Vector3(0, 0, dir * LaneController.laneWidthInWU);
    }

    private void MoveForward()
    {
        var movVec = new Vector3(Time.deltaTime * speedFactor, 0, 0);

        if (IsPlayerCar)
            transform.position += movVec;
        else
            transform.position -= movVec;
    }
}
