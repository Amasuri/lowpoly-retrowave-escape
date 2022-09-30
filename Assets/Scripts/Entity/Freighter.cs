using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freighter : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        var pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, pos.z * 1.0015f);
    }
}
