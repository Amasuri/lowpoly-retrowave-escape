using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    static public List<CarController> allCars = new List<CarController>();

    public Collider collider;
    public Rigidbody rigidbody;

    private float TurningGoalLane = 0f;
    private readonly float[] PossibleLanes = { -4f, -2f, 0f, 2f, 4f };

    public bool IsReverse { get; private set; }
    public bool IsPlayerCar;
    public bool IsTurningNow;

    public bool HasThisCarCollided { get; private set; }

    static public float speedFactorCurrent => (startSpeed + speedIncrease) > speedLimit ? speedLimit : (startSpeed + speedIncrease); //not pure speed because delta * speedfactor
    public const float startSpeed = 15f;
    public const float speedLimit = 45f; //30s limit; 60s would be ~75f
    static private float speedIncrease => RunTimer.TimeSinceLastRunStartSec / 1f; //approx 60f increase per minute

    private const float rotAmplitude = 3f;
    private const int reverseTurnSpeed = 5; //bigger => faster. 10 is slow, how is was in alpha, 5 is fast

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
        if (collision.gameObject.GetComponent<CarController>() != null)
            HandleCarToCarCollision(collision);
        else if (collision.gameObject.GetComponent<TerrainData>() != null)
            HandleCarToTerrainCollision(collision);
    }

    private void HandleCarToCarCollision(Collision collision)
    {
        HasThisCarCollided = true;
        if (IsPlayerCar)
            LaneController.RecordThatPlayerCollided();

        var sound = gameObject.GetComponent<CarCollisionSoundController>();
        if (sound != null)
            sound.PlayCollisionSound();

        //Jerk the car a but upwards and to the side; add counterforce to it and the other car
        transform.Rotate(new Vector3(10, 0, 0));
        var inertiaSum = startSpeed + speedFactorCurrent;
        var xInertia = IsPlayerCar ? -inertiaSum : inertiaSum;
        rigidbody.AddForce(new Vector3(xInertia, 2, Random.Range(-1f, 1f)), ForceMode.Impulse);
        collision.rigidbody.AddForce(new Vector3(-xInertia, 2, Random.Range(-1f, 1f)), ForceMode.Impulse);
    }

    private void HandleCarToTerrainCollision(Collision collision)
    {
        var terr = collision.gameObject.GetComponent<TerrainData>();

        //Normal terrain just acts as usual. It's outer terrain that needs action
        if (!terr.IsOuterTerrain || HasThisCarCollided)
            return;

        HasThisCarCollided = true;
        if (IsPlayerCar)
            LaneController.RecordThatPlayerCollided();

        var inertiaSum = startSpeed + speedFactorCurrent;
        var xInertia = inertiaSum;//IsPlayerCar ? inertiaSum : -inertiaSum;
        rigidbody.AddForce(new Vector3(xInertia, 2, transform.position.z), ForceMode.Impulse);
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

    /// <summary>
    /// Shouldn't be called on scene start, since it resets the player car after it has been set.
    /// Rather, it should be called before soft scene reload, to wipe the static variable
    /// </summary>
    static public void ResetAllCarsBeforeSceneWipe()
    {
        CarController.allCars = new List<CarController>();
    }

    static public void DestroyAllCarsBeforePlayButton()
    {
        foreach (var car in allCars)
        {
            if (car != null)
                Destroy(car.gameObject, 1f);
        }
    }

    public void StartChangingLane(bool toLeft = true)
    {
        var dir = toLeft ? 1 : -1;

        IsTurningNow = true;
        TurningGoalLane = transform.position.z + dir * LaneController.laneWidthInWU;
    }

    public void MakeReverse()
    {
        IsReverse = true;
        transform.Rotate(new Vector3(0, 180, 0));
    }

    private void DoSmoothTurn()
    {
        //Smooth lane change
        var signedDelta = transform.position.z - TurningGoalLane;
        transform.position += new Vector3(0, 0, -signedDelta / reverseTurnSpeed);

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
        if (IsPlayerCar)
        {
            var movVec = new Vector3(Time.fixedDeltaTime * speedFactorCurrent, 0, 0);
            transform.position += movVec;
        }
        else
        {
            var movVec = new Vector3(Time.fixedDeltaTime * startSpeed, 0, 0);
            if(!IsReverse)
                transform.parent.position += movVec;
            else
                transform.parent.position -= movVec;
        }
    }
}
