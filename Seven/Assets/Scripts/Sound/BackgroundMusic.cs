using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    public AudioSource audioSource;

    public bool loop;
    public float loopStartTime;
    [Tooltip("Baseline volume to be used to moderate volume control. Should essentially be the same as what you would set the audiosource volume to.")]
    [Range(0, 1)]
    public float originalVolume;

    void Awake()
    {
        if (this.audioSource == null)
        {
            this.audioSource = GetComponent<AudioSource>();
        }
        originalVolume = audioSource.volume;
        // Don't use the audioSource's builtin loop. Update() will manually loop the audio clip
        this.audioSource.loop = false;
        SetVolume();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.loop && !this.audioSource.isPlaying && !MenuManager.GAME_IS_OVER)
        {
            this.audioSource.time = this.loopStartTime;
            this.audioSource.Play();
        }
    }

    public void SetVolume()
    {
        if (this.audioSource != null)
        {
            this.audioSource.volume = originalVolume * GameSettings.MASTER_VOLUME * GameSettings.MUSIC_VOLUME;
        }
    }
}
