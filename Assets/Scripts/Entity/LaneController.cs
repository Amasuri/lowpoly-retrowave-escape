using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneController : MonoBehaviour
{
    static public List<Transform> terrainIndex = new List<Transform>();

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

    public bool IsReverseCarSpawnBanned => reverseCarSpawnBannedForS > 0f;
    private float reverseCarSpawnBannedForS = 0f;

    //Timer
    private const float maxTrafficSpawnDelay = 3f;
    private float currentTrafficSpawnDelayDecrease => MathHelper.RemapAndLimitToRange(RunTimer.TimeSinceLastRunStartSec, 0f, 60f, 0f, 2f);
    private float currentTrafficSpawnDelay => maxTrafficSpawnDelay - currentTrafficSpawnDelayDecrease; //currently the range is 3s..1s

    private float leftBeforeNextTrafficSpawn;

    /// <summary>
    /// Calculates destroy time based on speed (and thus, time needed to travel one chunk distance).
    /// Max time is 12(+1)s, meanwhile max and min speed are 45 and 15 units, meaning 45/15 = 3 relation.
    /// By this logic, minimum destruction time is 12/3 = 4(+1)s
    /// </summary>
    private float dynamicTerrainChunkDestroyTimeS => 1f + maxTerrainChunkDestroyTimeS * (CarController.startSpeed / CarController.speedFactorCurrent); //Car
    private const float firstTerrainChunkDestroyTimeS = 8f; //first spawned, right after player spawn
    private const float maxTerrainChunkDestroyTimeS = 12f; //by inverse relation, min time would be 12/3 = 4f

    static public bool HasPlayerCollided { get; private set; }
    public bool HasPlayerBeenSpawned;

    // Start is called before the first frame update
    private void Start()
    {
        PlayerCrashEvent.OnPlayerCrash += SpawnPersistentTerrainOnPlayerCrash;

        current = this;
        terrainIndex = new List<Transform>();

        HasPlayerBeenSpawned = false;
        ResetLaneController();
    }

    // Update is called once per frame
    private void Update()
    {
        //Making sure cars don't crash unto you after difficult spawns (i.e. 4-per-row spawns)
        if (reverseCarSpawnBannedForS >= 0f)
            reverseCarSpawnBannedForS -= Time.deltaTime;

        //Traffic spawning
        leftBeforeNextTrafficSpawn -= Time.deltaTime;
        if(leftBeforeNextTrafficSpawn <= 0f)
        {
            leftBeforeNextTrafficSpawn = currentTrafficSpawnDelay;
            Debug.Log("Car spawned after " + currentTrafficSpawnDelay + "s");
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

    /// <summary>
    /// It's also called on PlayButton as well as Scene start, so keep that in mind.
    /// </summary>
    public void ResetLaneController()
    {
        HasPlayerCollided = false;
        leftBeforeNextTrafficSpawn = currentTrafficSpawnDelay;
        Debug.Log("Car spawned after " + currentTrafficSpawnDelay + "s");
        TimesTerrainWasSpawned = 0;
        reverseCarSpawnBannedForS = 0f;

        SpawnNextTerrainChunk(first: true);
    }

    public static void RecordThatPlayerCollided()
    {
        //Order of call is such that it first does things on crash, and then records the crash
        PlayerCrashEvent.CallPlayerCrashEvent();
        HasPlayerCollided = true;
    }

    public void SpawnPlayerCar()
    {
        var pCar = Instantiate(PlayerCar) as Transform;
        HasPlayerBeenSpawned = true;
    }

    public void DestroyAllTerrainAndCarsBeforePlayButton()
    {
        CarController.DestroyAllCarsBeforePlayButton();

        foreach (var terr in terrainIndex)
        {
            if(terr != null)
                Destroy(terr.gameObject, 1f);
        }
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
        //4-per-row spawns are tricky, because other cars may crash unto you while you go through the gap
        BanReverseCarSpawn();

        //Get free lane
        var exitLane = (int)(GetRandomLane() / 2);

        //For all five lanes from leftmost to central to rightmost
        for (int i = -2; i < 2 + 1; i++)
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

    private void BanReverseCarSpawn()
    {
        reverseCarSpawnBannedForS = currentTrafficSpawnDelay * 1.5f;
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
            terrainIndex.Add(newTerrainChunk);
            Destroy(newTerrainChunk.gameObject, firstTerrainChunkDestroyTimeS);
        }

        var furtherNewTerrainChunk = Instantiate(TerrainPrefab) as Transform;
        furtherNewTerrainChunk.position = defaultTerrainPosition + new Vector3(nxtSpwnPosByPlX + terrainLength, 0, 0);
        terrainIndex.Add(furtherNewTerrainChunk);

        //Fix for the bug when on changed state (camera->car-based spawn) next spawn is based off old values for some reason
        var debugActualSpawnTime = maxTerrainChunkDestroyTimeS;
        if (nxtSpwnPosByPlX <= terrainLength * 3)
        {
            Destroy(furtherNewTerrainChunk.gameObject, maxTerrainChunkDestroyTimeS);
            debugActualSpawnTime = maxTerrainChunkDestroyTimeS;
        }
        else
        {
            Destroy(furtherNewTerrainChunk.gameObject, dynamicTerrainChunkDestroyTimeS);
            debugActualSpawnTime = dynamicTerrainChunkDestroyTimeS;
        }

        Debug.Log(maxTerrainChunkDestroyTimeS +"* ("+CarController.startSpeed +"/"+ CarController.speedFactorCurrent+")"+
            "="+ dynamicTerrainChunkDestroyTimeS + "(actual:" + debugActualSpawnTime + ") ("+RunTimer.TimeSinceLastRunStartSec+")s");

        TimesTerrainWasSpawned++;
    }

    private void SpawnPersistentTerrainOnPlayerCrash()
    {
        //Destroy "unpersitent" terrain
        foreach (var terrain in terrainIndex)
        {
            if(terrain != null)
                Destroy(terrain.gameObject, 1f);
        }

        //Bind player position to current car
        MonoBehaviour plCar = CarController.GetPlayerCar();
        var plPos = plCar.transform.position;

        var nxtSpwnPosByPlX = ((int)plPos.x / (int)terrainLength) * terrainLength;

        for (int i = -1; i <= 2; i++)
        {
            var newTerrainChunk = Instantiate(TerrainPrefab) as Transform;
            newTerrainChunk.position = defaultTerrainPosition + new Vector3(nxtSpwnPosByPlX + terrainLength * i, 0, 0);
            terrainIndex.Add(newTerrainChunk);
        }
    }

    private float GetRandomLane()
    {
        return laneWidthInWU * Random.Range(-2, 2+1);
    }

    private void OnDisable()
    {
        PlayerCrashEvent.OnPlayerCrash -= SpawnPersistentTerrainOnPlayerCrash;
    }

    private void OnDestroy()
    {
        PlayerCrashEvent.OnPlayerCrash -= SpawnPersistentTerrainOnPlayerCrash;
    }
}
