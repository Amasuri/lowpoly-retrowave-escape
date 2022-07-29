using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBreath : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        var sc = 1f + (0.1f * Mathf.Cos(Time.realtimeSinceStartup));

        transform.localScale = new Vector3(sc, sc, transform.localScale.z);
    }
}
