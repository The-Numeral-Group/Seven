using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

public class ActorSoundManager : MonoBehaviour
{
    public SoundAudioClip[] sounds;

    public static ActorSoundManager instance;

    private static Dictionary<string, float> soundTimerDictionary = new Dictionary<string, float>();

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

            if (s.hasCooldown)
            {
                soundTimerDictionary[s.name] = -s.audioClip.length;
            }
        }
    }

    private static bool CanPlaySound(SoundAudioClip sound)
    {
        if (soundTimerDictionary.ContainsKey(sound.name))
        {
            float lastTimePlayed = soundTimerDictionary[sound.name];
            if (lastTimePlayed + sound.audioClip.length < Time.time)
            {
                soundTimerDictionary[sound.name] = Time.time;
                return true;
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    private void UpdateLastPlayedSound(SoundAudioClip sound)
    {
        if (soundTimerDictionary.ContainsKey(sound.name))
        {
            soundTimerDictionary[sound.name] = Time.time - sound.audioClip.length;
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

        if (!CanPlaySound(s)) return;

        //Debug.Log(s.name);

        s.source.Play();
    }

    public void StopSound (string name)
    {
        SoundAudioClip s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("SoundManager.StopSound: Cannot find " + name);
            return;
        }

        UpdateLastPlayedSound(s);

        s.source.Stop();
    }

    public void PlaySoundAtClip (string name)
    {
        SoundAudioClip s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("SoundManager.StopSound: Cannot find " + name);
            return;
        }
        if (!CanPlaySound(s)) return;

        AudioSource.PlayClipAtPoint(s.audioClip, Vector2.zero);
    }
}
