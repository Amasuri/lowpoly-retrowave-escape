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
    private const float carSpawnOffsetXmin = 50f; //used to be 40
    private readonly float carSpawnOffsetXmax = terrainLength - 1f;

    public bool IsReverseCarSpawnBanned => reverseCarSpawnBannedForS > 0f;
    private float reverseCarSpawnBannedForS = 0f;

    public bool IsFourFormationBanned => FourFormationBannedForS > 0f;
    private float FourFormationBannedForS = 0f;

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

        if (FourFormationBannedForS >= 0f)
            FourFormationBannedForS -= Time.deltaTime;

        //Traffic spawning
        leftBeforeNextTrafficSpawn -= Time.deltaTime;
        if(leftBeforeNextTrafficSpawn <= 0f)
        {
            leftBeforeNextTrafficSpawn = currentTrafficSpawnDelay;
            Logger.Log("Car spawned after " + currentTrafficSpawnDelay + "s");
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
        Logger.Log("Car spawned after " + currentTrafficSpawnDelay + "s");
        TimesTerrainWasSpawned = 0;
        reverseCarSpawnBannedForS = 0f;
        FourFormationBannedForS = 0f;

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
        if (!HasPlayerBeenSpawned || HasPlayerCollided)
            pCar = CameraControllerRun.current;
        if (pCar == null)
            return;
        var pPos = pCar.transform.position;

        //Spawn offset is calculated depending on current time, but no more than max spawn offset
        var spawnOffset = MathHelper.Remap(RunTimer.TimeSinceLastRunStartSec, 0f, 60f, carSpawnOffsetXmin, carSpawnOffsetXmax);
        spawnOffset = spawnOffset > carSpawnOffsetXmax ? carSpawnOffsetXmax : spawnOffset;

        //If player has collided, spawn cars not up ahead, but behind
        if (HasPlayerCollided)
        {
            spawnOffset = -spawnOffset;
            Logger.Log("Spawning behind!");
        }

        //Decide on spawning pattern
        int chance = Random.Range(0, 100);

        //50% to spawn one car, each 25% to spawn three, 15% triangular or 10% four cars on the same row
        if (chance < 50)
            SpawnOneCarPerRow(pPos, spawnOffset);
        else if (chance >= 50 && chance < 75)
            SpawnThreeCarsPerRow(pPos, spawnOffset);
        else if (chance >= 75 && chance < 90)
            SpawnTriangularRow(pPos, spawnOffset);
        else if (chance >= 90)
            SpawnFourCarsPerRow(pPos, spawnOffset);
    }

    private void SpawnOneCarPerRow(Vector3 pPos, float spawnOffset)
    {
        //Instantiate
        var newTrafficCar = Instantiate(TrafficCar1) as Transform;
        newTrafficCar.position = new Vector3(pPos.x + spawnOffset, 0.02f, GetRandomLane());
    }

    private void SpawnFourCarsPerRow(Vector3 pPos, float spawnOffset)
    {
        if(IsFourFormationBanned)
        {
            Logger.Log("Four formation car spawn banned, spawn aborted!");
            return;
        }

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
        }
    }

    private void SpawnTriangularRow(Vector3 pPos, float spawnOffset)
    {
        //Not specifically a problematic formation, but there could be problems if player wanted to go between the cars
        BanReverseCarSpawn();

        //Turned out to be quite tricky with 4-formation patterns
        BanFourFormationSpawn();

        //If we count the whole formation; there's only three lanes available for car spawn;
        //that means only the left and center ones are eligible
        var formationStartLane = Random.Range(-2, 0 + 1) * laneWidthInWU;

        //Formation is continious: it means that there are no gaps between cars of the formation; but there's only three of them
        for (int i = 0; i < 2 + 1; i++)
        {
            //Lead car is up ahead
            var leadCarOffset = 0f;
            if (i == 1)
                leadCarOffset = spawnOffset / 5;

            //Instantiate
            var newTrafficCar = Instantiate(TrafficCar1) as Transform;
            newTrafficCar.position = new Vector3(pPos.x + spawnOffset + leadCarOffset + Random.Range(-1f, 1f), 0.02f, formationStartLane + i * laneWidthInWU);
        }
    }

    private void BanReverseCarSpawn()
    {
        reverseCarSpawnBannedForS = currentTrafficSpawnDelay * 1.5f;
    }

    private void BanFourFormationSpawn()
    {
        FourFormationBannedForS = currentTrafficSpawnDelay * 1.5f;
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

        Logger.Log("Terrain spawn info: "+maxTerrainChunkDestroyTimeS +"* ("+CarController.startSpeed +"/"+ CarController.speedFactorCurrent+")"+
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
