using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneController : MonoBehaviour
{
    private readonly Vector3 defaultTerrainPosition = new Vector3(0, 0, -5); //new Vector3(-terrainSpawnOffset, 0, -5);
    private const float terrainLength = 100f; //isn't hooked; change in terrain editor prefab

    private int TimesTerrainWasSpawned;

    public Transform TrafficCar1;
    public Transform TerrainPrefab;

    public const float laneWidthInWU = 2f;
    public const float carSpawnOffsetX = 40f;

    //Timer
    private const float maxTrafficSpawnDelay = 3f;
    private float currentTrafficSpawnDelay;

    static public bool HasPlayerCollided { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        currentTrafficSpawnDelay = maxTrafficSpawnDelay;
        TimesTerrainWasSpawned = 0;

        SpawnNextTerrainChunk(first: true);
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

        //Terrain spawning (accuracy-dependent, so under fixed update)
        var pCar = CarController.GetPlayerCar();
        if (pCar == null)
            return;
        var pPos = pCar.transform.position;

        if ((pPos.x / terrainLength) > TimesTerrainWasSpawned)
            SpawnNextTerrainChunk();
    }

    private void FixedUpdate()
    {
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
        newTrafficCar.position = new Vector3(pPos.x + carSpawnOffsetX, pPos.y, GetRandomLane());
    }

    private void SpawnNextTerrainChunk(bool first = false)
    {
        var plCar = CarController.GetPlayerCar();
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
