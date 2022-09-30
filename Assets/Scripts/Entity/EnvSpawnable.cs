using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvSpawnable : MonoBehaviour
{
    public EnvType envType;
    public enum EnvType
    {
        Hut,
        Skyscraper,
        Bird,
        Plane,
        Freighter
    }

    private void Start()
    {
        int chance = Random.Range(0, 100);

        int spawnChance = GetSpawnChanceByType();

        //Basically ~20% chance
        if (chance >= spawnChance)
        {
            gameObject.SetActive(false);
            Debug.Log(envType + " not spawned!");
        }
        else
        {
            Debug.Log(envType + " spawned!");
        }
    }

    private int GetSpawnChanceByType()
    {
        switch (envType)
        {
            case EnvType.Hut:
                return 20;

            case EnvType.Skyscraper:
                return 20;

            case EnvType.Bird:
                return 10;

            case EnvType.Plane:
                return 10;

            case EnvType.Freighter:
                return 20;

            default:
                return 0;
        }
    }
}
