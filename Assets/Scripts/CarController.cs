using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    static public List<CarController> allCars = new List<CarController>();

    public bool IsPlayerCar;

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
        MoveForward();
    }

    private void MoveForward()
    {
        var movVec = new Vector3(Time.deltaTime, 0, 0);

        if (IsPlayerCar)
            transform.position += movVec;
        else
            transform.position -= movVec;
    }

    public void MoveToLeft()
    {
        transform.position += new Vector3(0, 0, LaneController.laneWidthInWU);
    }

    public void MoveToRight()
    {
        transform.position += new Vector3(0, 0, -LaneController.laneWidthInWU);
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
}
