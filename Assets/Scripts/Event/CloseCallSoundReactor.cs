using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCallSoundReactor : MonoBehaviour
{
    public AudioSource audio;

    private void Start()
    {
        CloseCallEvent.OnCloseCall += PlayBonus;
    }

    private void PlayBonus()
    {
        audio.Play();
    }

    private void OnDestroy()
    {
        CloseCallEvent.OnCloseCall -= PlayBonus;
    }

    private void OnDisable()
    {
        CloseCallEvent.OnCloseCall -= PlayBonus;
    }
}
