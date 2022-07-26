using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSoundController : MonoBehaviour
{
    public bool IsPlayerCar;

    public AudioSource soundPlayer;

    public AudioClip collisionClip;

    private AudioClip previousTrack;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void PlayCollisionSound()
    {
        soundPlayer.clip = collisionClip;
        soundPlayer.Play();
    }
}
