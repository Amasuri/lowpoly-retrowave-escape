using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hut : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        int chance = Random.Range(0, 100);

        //Basically ~80% chance
        if (chance >= 20)
        {
            gameObject.SetActive(false);
            Debug.Log("Hut not spawned!");
        }
        else
        {
            Debug.Log("Hut spawned!");
        }
    }
}
