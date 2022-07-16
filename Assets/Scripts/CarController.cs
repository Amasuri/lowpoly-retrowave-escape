using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    static public List<CarController> allCars = new List<CarController>();

    public Collider collider;
    public Rigidbody rigidbody;

    private float TurningGoalLane = 0f;

    public bool IsPlayerCar;
    public bool IsTurningNow;
    public bool HasThisCarCollided { get; private set; }

    static private float speedFactor => 15f + speedIncrease;
    static private float speedIncrease => RunTimer.TimeSinceLastRunStartSec / 1f; //approx 60f increase per minute

    private const float rotAmplitude = 3f;

    // Start is called before the first frame update
    private void Start()
    {
        allCars.Add(this);
        HasThisCarCollided = false;
        IsTurningNow = false;

        //Make sure passing cars get KO'd
        if (!IsPlayerCar)
            Destroy(gameObject, 15);
    }

    private void FixedUpdate()
    {
        if (HasThisCarCollided)
            return;

        MoveForward();
        if (IsTurningNow)
            DoSmoothTurn();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Make sure you write only for car collisions
        if (collision.gameObject.GetComponent<CarController>() == null)
            return;

        HasThisCarCollided = true;

        if (IsPlayerCar)
            LaneController.RecordThatPlayerCollided();

        //Jerk the car a but upwards and to the side
        transform.Rotate(new Vector3(10, 0, 0));
        rigidbody.AddForce(new Vector3(Random.Range(-1f,1f), 2, 0), ForceMode.Impulse);
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

    public void StartChangingLane(bool toLeft = true)
    {
        var dir = toLeft ? 1 : -1;

        IsTurningNow = true;
        TurningGoalLane = transform.position.z + dir * LaneController.laneWidthInWU;
    }

    private void DoSmoothTurn()
    {
        //Smooth lane change
        var signedDelta = transform.position.z - TurningGoalLane;
        transform.position += new Vector3(0, 0, -signedDelta / 10);

        //Smooth rotation dip
        var rot = Mathf.Cos(signedDelta + (1 * Mathf.Sign(signedDelta)));
        transform.Rotate(new Vector3(0, rot * rotAmplitude * Mathf.Sign(-signedDelta), 0));

        //If very close to the right lane, do the snap
        if (Mathf.Abs(signedDelta) <= LaneController.laneWidthInWU / 25)
        {
            IsTurningNow = false;
            transform.position = new Vector3(transform.position.x, transform.position.y, TurningGoalLane);

            //And fix rotation
            ;
        }
    }

    private void MoveForward()
    {
        var movVec = new Vector3(Time.fixedDeltaTime * speedFactor, 0, 0);

        if (IsPlayerCar)
            transform.position += movVec;
        else
            transform.position -= movVec;
    }
}
