using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

public class ActorSoundManager : MonoBehaviour
{
    public SoundAudioClip[] sounds;

    public static ActorSoundManager instance;

    void Awake()
    {
        // This is for preventing sound getting cutoff after scene transition.
        // Also, this prevents having more thatn one soundManager inside DontDestroyOnLoad.
        /*if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }*/

        foreach (SoundAudioClip s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.playOnAwake = s.playOnAwake;
            s.source.loop = s.loop;
        }
    }

    public void PlaySound (string name)
    {
        SoundAudioClip s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("SoundManager.PlaySound: Cannot find " + name);
            return;
        }
        s.source.Play();
    }
}
