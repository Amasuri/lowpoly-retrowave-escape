using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarExhaustEffect : MonoBehaviour
{
    public Light light;

    private const float ExhaustCDmax = 0.1f;
    private float ExhaustCDnow = 0f;

    private void Start()
    {
        CarExhaustSoundController.OnEngineExhaust += StartExhaustEventEffect;

        light.enabled = false;
        ExhaustCDnow = 0f;
    }

    private void Update()
    {
        if (ExhaustCDnow > 0f)
        {
            ExhaustCDnow -= Time.deltaTime;
            if (ExhaustCDnow <= 0f)
                light.enabled = false;
        }
    }

    private void StartExhaustEventEffect()
    {
        light.enabled = true;
        ExhaustCDnow = ExhaustCDmax;
    }

    private void OnDestroy()
    {
        CarExhaustSoundController.OnEngineExhaust -= StartExhaustEventEffect;
    }

    private void OnDisable()
    {
        CarExhaustSoundController.OnEngineExhaust -= StartExhaustEventEffect;
    }
}
