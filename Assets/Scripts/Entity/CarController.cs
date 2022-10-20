using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    static public List<CarController> allCars = new List<CarController>();

    public Collider collider;
    public Rigidbody rigidbody;

    /// <summary>
    /// Only makes sense for player (because only player changes positions)
    /// </summary>
    private int NextStrictLanePosition;
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

    private float defaultRotation;

    // Start is called before the first frame update
    private void Start()
    {
        allCars.Add(this);
        HasThisCarCollided = false;
        IsTurningNow = false;

        NextStrictLanePosition = 0;

        //Make sure passing cars get KO'd
        if (!IsPlayerCar)
            Destroy(gameObject, 15f); //15 default, 10-12f they sometimes disappear in the sky after hit & flying from back to front (not as fun to see them fly)

        //10% chance to get new car be reverse-moving
        if (Random.Range(0, 100) <= 10 && !IsPlayerCar)
        {
            this.MakeReverse();
        }

        defaultRotation = transform.rotation.eulerAngles.y;
    }

    private void FixedUpdate()
    {
        if (HasThisCarCollided)
            return;

        MoveForward();
        if (IsTurningNow)
            DoSmoothTurn();
        else if (transform.rotation.eulerAngles.y != defaultRotation)
            CorrectCarRotation();
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
        NextStrictLanePosition += dir * (int)LaneController.laneWidthInWU;
        TurningGoalLane = NextStrictLanePosition; //transform.position.z + dir * LaneController.laneWidthInWU;
    }

    private void MakeReverse()
    {
        if(LaneController.current.IsReverseCarSpawnBanned)
        {
            Logger.Log("Reverse car spawn banned, reversing aborted!");
            return;
        }

        IsReverse = true;
        transform.Rotate(new Vector3(0, 180, 0));
    }

    private void DoSmoothTurn()
    {
        var signedDelta = transform.position.z - TurningGoalLane;

        DoSmoothTurnPosition(signedDelta);
        DoSmoothTurnRotation(signedDelta);
        DoSmoothTurnSnapping(signedDelta);
    }

    private void DoSmoothTurnPosition(float signedDelta)
    {
        transform.position += new Vector3(0, 0, -signedDelta / reverseTurnSpeed);
    }

    private void DoSmoothTurnRotation(float signedDelta)
    {
        //Sanity check: Should always pursue rot 0 if less than 50% of laneWU.
        if (Mathf.Abs(signedDelta) <= LaneController.laneWidthInWU * 0.50f)
        {
            CorrectCarRotation();
        }

        //The below case works well when delta <= laneWU, but falls apart when delta is LaneWU*1f..2f, resulting in "drifting" car.
        //Hence the need for above sanity check
        else
        {
            //Smooth rotation dip based on cosine
            var rot = Mathf.Cos(signedDelta + (1 * Mathf.Sign(signedDelta)));
            transform.Rotate(new Vector3(0, rot * rotAmplitude * Mathf.Sign(-signedDelta), 0));
        }
    }

    private void DoSmoothTurnSnapping(float signedDelta)
    {
        //If very close to the right lane, do the snap
        if (Mathf.Abs(signedDelta) <= LaneController.laneWidthInWU / 25)
        {
            IsTurningNow = false;
            transform.position = new Vector3(transform.position.x, transform.position.y, TurningGoalLane);

            //And fix rotation
            ; //(isn't needed by current design, it's done as one of "correct car" sanity check things)
        }
    }

    private void CorrectCarRotation()
    {
        var def = defaultRotation;
        var curr = transform.rotation.eulerAngles.y;
        var signedDelta = def - curr;

        if(Mathf.Abs(signedDelta) <= 0.01f)
        {
            return;
        }
        else if(Mathf.Abs(signedDelta) <= 1f)
        {
            var correction = signedDelta;
            transform.Rotate(new Vector3(0, correction, 0));
        }
        else
        {
            var correction = (def - curr) / 10;
            transform.Rotate(new Vector3(0, correction, 0));
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

    private void OnDestroy()
    {
        allCars.Remove(this);
        Logger.Log("Car despawned! Cars left: " + allCars.Count);
    }
}
