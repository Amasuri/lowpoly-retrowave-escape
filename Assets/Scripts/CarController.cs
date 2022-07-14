using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    static public List<CarController> allCars = new List<CarController>();

    public Collider collider;

    public bool IsPlayerCar;
    private bool hasThisCarCollided = false;

    private const float speedFactor = 5f;

    // Start is called before the first frame update
    private void Start()
    {
        allCars.Add(this);

        if (!IsPlayerCar)
            Destroy(gameObject, 10);
    }

    // Update is called once per frame
    private void Update()
    {
        KeepAboveFloor();

        if(!hasThisCarCollided)
            MoveForward();
    }

    private void OnCollisionEnter(Collision collision)
    {
        hasThisCarCollided = true;

        if (IsPlayerCar)
            LaneController.CollidePlayer();
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

    private void KeepAboveFloor()
    {
        if (transform.position.y < 0f)
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }
}
