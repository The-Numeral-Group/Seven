using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

// Doc: https://docs.google.com/document/d/12IDY_Z5j9Umr49aE8aMV6Ke6XBz1HQj-TMkL8JfRF74/edit
public class ActorSoundManager : MonoBehaviour
{
    public SoundAudioClip[] sounds;

    public static ActorSoundManager instance;

    private static Dictionary<string, float> soundTimerDictionary = new Dictionary<string, float>();

    void Awake()
    { 
        foreach (SoundAudioClip s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;

            s.source.volume = s.volume * GameSettings.MASTER_VOLUME * GameSettings.SFX_VOLUME;
            s.source.pitch = s.pitch;

            s.source.playOnAwake = s.playOnAwake;
            s.source.loop = s.loop;

            if (s.hasCooldown)
            {
                soundTimerDictionary[s.name] = -s.audioClip.length;
            }

            if (s.playOnAwake)
            {
                s.source.Play();
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

    public void PlaySound (string name, float pitchMin = 1, float pitchMax = 1)
    {
        SoundAudioClip s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("SoundManager.PlaySound: Cannot find " + name);
            return;
        }

        if (!CanPlaySound(s)) return;

        s.source.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);
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

    public IEnumerator muteSoundForDuration (string name, float duration)
    {
        SoundAudioClip s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("SoundManager.StopSound: Cannot find " + name);
        }
        s.source.mute = true;
        yield return new WaitForSeconds(duration);
        s.source.mute = false;
    }

    //This function is called from the sound settings menu to update the volume of the audio clips an actor has.
    //I am noit sure if updating all the volumes at once, or only when they are played is better.
    //I think the size of audio clips getting updated in a scene is small enough that I think it is fine.
    public void SetClipVolume()
    {
        foreach(SoundAudioClip s in sounds)
        {
            if (s.source != null)
            {
                s.source.volume = s.volume * GameSettings.MASTER_VOLUME * GameSettings.SFX_VOLUME;
            }
        }
    }
}
