using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneController : MonoBehaviour
{
    private readonly Vector3 defaultTerrainPosition = new Vector3(0, 0, -5); //new Vector3(-terrainSpawnOffset, 0, -5);
    private const float terrainLength = 100f; //isn't hooked; change in terrain editor prefab

    private int TimesTerrainWasSpawned;

    static public LaneController current;

    public Transform PlayerCar;
    public Transform TrafficCar1;
    public Transform TerrainPrefab;

    public const float laneWidthInWU = 2f;
    private const float carSpawnOffsetXmin = 40f;
    private readonly float carSpawnOffsetXmax = terrainLength - 1f;

    //Timer
    private const float maxTrafficSpawnDelay = 3f;
    private float currentTrafficSpawnDelay;

    static public bool HasPlayerCollided { get; private set; }
    public bool HasPlayerBeenSpawned;

    // Start is called before the first frame update
    private void Start()
    {
        current = this;

        HasPlayerBeenSpawned = false;
        ResetLaneController();
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
        //Bind player position to either current cam or current car
        MonoBehaviour pCar = CarController.GetPlayerCar();
        if(!HasPlayerBeenSpawned)
            pCar = CameraControllerRun.current;
        if (pCar == null)
            return;

        var pPos = pCar.transform.position;

        if ((pPos.x / terrainLength) > TimesTerrainWasSpawned)
            SpawnNextTerrainChunk();
    }

    public void ResetLaneController()
    {
        HasPlayerCollided = false;
        currentTrafficSpawnDelay = maxTrafficSpawnDelay;
        TimesTerrainWasSpawned = 0;

        SpawnNextTerrainChunk(first: true);
    }

    public static void RecordThatPlayerCollided()
    {
        HasPlayerCollided = true;
    }

    public void SpawnPlayerCar()
    {
        var pCar = Instantiate(PlayerCar) as Transform;
        HasPlayerBeenSpawned = true;
    }

    private void SpawnNewTraffic()
    {
        //Try to get player car pos safely
        //Bind player position to either current cam or current car
        MonoBehaviour pCar = CarController.GetPlayerCar();
        if (!HasPlayerBeenSpawned)
            pCar = CameraControllerRun.current;
        if (pCar == null)
            return;
        var pPos = pCar.transform.position;

        //Spawn offset is calculated depending on current time, but no more than max spawn offset
        var spawnOffset = MathHelper.Remap(RunTimer.TimeSinceLastRunStartSec, 0f, 60f, carSpawnOffsetXmin, carSpawnOffsetXmax);
        spawnOffset = spawnOffset > carSpawnOffsetXmax ? carSpawnOffsetXmax : spawnOffset;

        //Decide on spawning pattern
        int chance = Random.Range(0, 100);

        //50% to spawn one car, each 25% to spawn three or four cars on the same row
        if(chance < 50)
            SpawnOneCarPerRow(pPos, spawnOffset);
        else if (chance >= 50 && chance <= 75)
            SpawnFourCarsPerRow(pPos, spawnOffset);
        else
            SpawnThreeCarsPerRow(pPos, spawnOffset);
    }

    private void SpawnOneCarPerRow(Vector3 pPos, float spawnOffset)
    {
        //Instantiate
        var newTrafficCar = Instantiate(TrafficCar1) as Transform;
        newTrafficCar.position = new Vector3(pPos.x + spawnOffset, 0.02f, GetRandomLane());

        //10% chance to get new car be reverse-moving
        if (Random.Range(0, 100) <= 10)
        {
            var car = newTrafficCar.GetChild(0).GetComponent<CarController>();
            car.MakeReverse();
        }
    }

    private void SpawnFourCarsPerRow(Vector3 pPos, float spawnOffset)
    {
        //Get free lane
        var exitLane = (int)(GetRandomLane()/2);

        //For all five lanes from leftmost to central to rightmost
        for (int i = -2; i < 2+1; i++)
        {
            if (i == exitLane) continue;

            //Instantiate
            var newTrafficCar = Instantiate(TrafficCar1) as Transform;
            newTrafficCar.position = new Vector3(pPos.x + spawnOffset + Random.Range(-1f, 1f), 0.02f, i * laneWidthInWU);

            //10% chance to get new car be reverse-moving
            if (Random.Range(0, 100) <= 10)
            {
                var car = newTrafficCar.GetChild(0).GetComponent<CarController>();
                car.MakeReverse();
            }
        }
    }

    private void SpawnThreeCarsPerRow(Vector3 pPos, float spawnOffset)
    {
        //Get free lanes (there's 2 of them)
        var exitLane1 = (int)(GetRandomLane() / 2);
        var exitLane2 = (int)(GetRandomLane() / 2);
        while(exitLane1 == exitLane2)
            exitLane2 = (int)(GetRandomLane() / 2);

        //For all five lanes from leftmost to central to rightmost
        for (int i = -2; i < 2 + 1; i++)
        {
            if (i == exitLane1 || i == exitLane2) continue;

            //Instantiate
            var newTrafficCar = Instantiate(TrafficCar1) as Transform;
            newTrafficCar.position = new Vector3(pPos.x + spawnOffset + Random.Range(-1f, 1f), 0.02f, i * laneWidthInWU);

            //10% chance to get new car be reverse-moving
            if (Random.Range(0, 100) <= 10)
            {
                var car = newTrafficCar.GetChild(0).GetComponent<CarController>();
                car.MakeReverse();
            }
        }
    }

    private void SpawnNextTerrainChunk(bool first = false)
    {
        //Bind player position to either current cam or current car
        MonoBehaviour plCar = CarController.GetPlayerCar();
        if (!HasPlayerBeenSpawned)
            plCar = CameraControllerRun.current;

        var plPos = new Vector3(0, 0, 0);
        if (plCar != null)
            plPos = plCar.transform.position;

        var nxtSpwnPosByPlX = ((int)plPos.x / (int)terrainLength) * terrainLength;

        if (first)
        {
            var newTerrainChunk = Instantiate(TerrainPrefab) as Transform;
            newTerrainChunk.position = defaultTerrainPosition + new Vector3(nxtSpwnPosByPlX, 0, 0);
            Destroy(newTerrainChunk.gameObject, 8f);
        }

        var furtherNewTerrainChunk = Instantiate(TerrainPrefab) as Transform;
        furtherNewTerrainChunk.position = defaultTerrainPosition + new Vector3(nxtSpwnPosByPlX + terrainLength, 0, 0);
        Destroy(furtherNewTerrainChunk.gameObject, 12f);

        TimesTerrainWasSpawned++;
    }

    private float GetRandomLane()
    {
        return laneWidthInWU * Random.Range(-2, 2+1);
    }
}
