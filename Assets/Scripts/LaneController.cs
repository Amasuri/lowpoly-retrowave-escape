using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneController : MonoBehaviour
{
    public Transform TrafficCar1;

    public const float laneWidthInWU = 2f;
    public const float spawnOffsetX = 40f;

    //Timer
    private const float maxSpawnDelay = 3f;
    private float currentSpawnDelay;

    static public bool HasPlayerCollided { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        currentSpawnDelay = maxSpawnDelay;
    }

    // Update is called once per frame
    private void Update()
    {
        currentSpawnDelay -= Time.deltaTime;
        if(currentSpawnDelay <= 0f)
        {
            currentSpawnDelay = maxSpawnDelay;
            SpawnNewTraffic();
        }
    }

    public static void CollidePlayer()
    {
        HasPlayerCollided = true;
    }

    private void SpawnNewTraffic()
    {
        var pCar = CarController.GetPlayerCar();
        if (pCar == null)
            return;

        var pPos = pCar.transform.position;

        var newTrafficCar = Instantiate(TrafficCar1) as Transform;
        newTrafficCar.position = new Vector3(pPos.x + spawnOffsetX, pPos.y, GetRandomLane());
    }

    private float GetRandomLane()
    {
        return laneWidthInWU * Random.Range(-2, 2+1);
    }
}
