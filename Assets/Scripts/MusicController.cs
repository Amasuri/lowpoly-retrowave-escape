using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource musicPlayer;

    public AudioClip runnerMusic;
    public AudioClip loadingMusic;

    private AudioClip previousTrack;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        //Audio setting switcher
        if(!SystemSettings.MUSIC_ON && musicPlayer.isPlaying)
        {
            musicPlayer.Stop();
        }
        else if(SystemSettings.MUSIC_ON && !musicPlayer.isPlaying)
        {
            musicPlayer.Play();
        }

        //Switch & play tracks based on current context
        if (LaneController.current.HasPlayerBeenSpawned)
            musicPlayer.clip = runnerMusic;
        else
            musicPlayer.clip = loadingMusic;

        if (previousTrack != musicPlayer.clip)
            musicPlayer.Play();

        previousTrack = musicPlayer.clip;
    }
}
