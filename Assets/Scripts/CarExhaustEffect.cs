using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarExhaustEffect : MonoBehaviour
{
    public Light light;

    private const float ExhaustCDmax = 0.3f / 2;
    private float ExhaustCDnow = 0f;

    //private readonly Dictionary<float, bool> blinkTimeline = new Dictionary<float, bool>
    //{
    //    { 0.3f, false }, //0.3-0.2f - true;  0.2f- -> false
    //    { 0.2f, true },  //0.2-0.1f - false; 0.1f- -> true
    //    { 0.1f, false }, //0.1-0.0f - true;  0.0f- -> false
    //};

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

            //Compare blink to blink timeline
            if (ExhaustCDnow <= 0.3f / 2 && ExhaustCDnow >= 0.2f / 2)
                light.enabled = true;
            else if (ExhaustCDnow <= 0.2f / 2 && ExhaustCDnow >= 0.1f / 2)
                light.enabled = false;
            else if (ExhaustCDnow <= 0.1f / 2 && ExhaustCDnow >= 0.0f / 2)
                light.enabled = true;

            //Unconditionally off by time it reaches zero
            if (ExhaustCDnow <= 0f)
            {
                light.enabled = false;
            }
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
