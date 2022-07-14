using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneController : MonoBehaviour
{
    public Transform TrafficCar1;

    public const float laneWidthInWU = 2f;

    private const float maxSpawnDelay = 3f;
    private float currentSpawnDelay;

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

    private void SpawnNewTraffic()
    {
        var pCar = CarController.GetPlayerCar();
        if (pCar == null)
            return;

        var pPos = pCar.transform.position;

        var newTrafficCar = Instantiate(TrafficCar1) as Transform;
        newTrafficCar.position = pPos + new Vector3(10f, 0, laneWidthInWU);
    }
}
