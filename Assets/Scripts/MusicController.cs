using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource musicPlayer;

    public AudioClip runnerMusic;
    public AudioClip loadingMusic;

    private AudioClip previousTrack;

    private static bool _DebugMute = false;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (LaneController.current.HasPlayerBeenSpawned)
            musicPlayer.clip = runnerMusic;
        else
            musicPlayer.clip = loadingMusic;

        if (previousTrack != musicPlayer.clip)
            musicPlayer.Play();

        //debug while listening to rave music
        if (Input.GetKeyDown(KeyCode.M))
            _DebugMute = !_DebugMute;
        if (_DebugMute && musicPlayer.isPlaying)
            musicPlayer.Stop();

        previousTrack = musicPlayer.clip;
    }
}
