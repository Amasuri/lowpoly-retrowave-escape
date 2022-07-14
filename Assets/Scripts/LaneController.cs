using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneController : MonoBehaviour
{
    private readonly Vector3 defaultTerrainPosition = new Vector3(-10, 0, -500);
    private int TimesTerrainWasSpawned;

    public Transform TrafficCar1;
    public Transform Terrain;

    public const float laneWidthInWU = 2f;
    public const float spawnOffsetX = 40f;

    //Timer
    private const float maxTrafficSpawnDelay = 3f;
    private float currentTrafficSpawnDelay;

    static public bool HasPlayerCollided { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        currentTrafficSpawnDelay = maxTrafficSpawnDelay;
        TimesTerrainWasSpawned = 0;

        SpawnNextTerrainChunk();
    }

    // Update is called once per frame
    private void Update()
    {
        //Traffic spawning
        currentTrafficSpawnDelay -= Time.deltaTime;
        if(currentTrafficSpawnDelay <= 0f)
        {
            currentTrafficSpawnDelay = maxTrafficSpawnDelay;
            SpawnNewTraffic();
        }

        //Terrain spawning
        var pCar = CarController.GetPlayerCar();
        if (pCar == null)
            return;
        var pPos = pCar.transform.position;

        if (((pPos.x + 100f) / 1000f) > TimesTerrainWasSpawned)
            SpawnNextTerrainChunk();
    }

    public static void RecordThatPlayerCollided()
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

    private void SpawnNextTerrainChunk()
    {
        var pCar = CarController.GetPlayerCar();
        if (pCar == null)
            return;

        var pPos = pCar.transform.position;

        var newTerrainChunk = Instantiate(Terrain) as Transform;
        newTerrainChunk.position = defaultTerrainPosition + new Vector3(pPos.x, 0, 0);

        TimesTerrainWasSpawned++;
    }

    private float GetRandomLane()
    {
        return laneWidthInWU * Random.Range(-2, 2+1);
    }
}
