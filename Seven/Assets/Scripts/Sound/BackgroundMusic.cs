using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource audioSource;

    public bool loop;

    public float loopStartTime;

    // Start is called before the first frame update
    void Start()
    {
        this.audioSource = this.GetComponent<AudioSource>();
        // Don't use the audioSource's builtin loop. Update() will manually loop the audio clip
        this.audioSource.loop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.loop && !this.audioSource.isPlaying)
        {
            this.audioSource.time = this.loopStartTime;
            this.audioSource.Play();
        }
    }
}
