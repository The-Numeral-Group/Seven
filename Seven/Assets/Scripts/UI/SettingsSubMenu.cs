using UnityEngine;
using UnityEngine.UI;

//Used with the settings sub menu within the pause menu
public class SettingsSubMenu : SubMenu
{
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider ambienceVolume;
    public Slider sfxVolume; 

    public void init()
    {
        masterVolume.value = GameSettings.MASTER_VOLUME;
        musicVolume.value = GameSettings.MUSIC_VOLUME;
        ambienceVolume.value = GameSettings.AMBIENT_VOLUME;
        sfxVolume.value = GameSettings.SFX_VOLUME;
        SetBackgroundMusic();
        SetSFXSound();
    }
    public void SetMasterVolume()
    {
        GameSettings.MASTER_VOLUME = masterVolume.value;
        SetBackgroundMusic();
        SetSFXSound();
    }
    public void SetMusicVolume()
    {
        GameSettings.MUSIC_VOLUME = musicVolume.value;
        SetBackgroundMusic();
    }
    public void SetAmbientVolume()
    {
        GameSettings.AMBIENT_VOLUME = ambienceVolume.value;
    }
    public void SetSFXVolume()
    {
        GameSettings.SFX_VOLUME = sfxVolume.value;
        SetSFXSound();
    }
    void SetBackgroundMusic()
    {
        var backgroundmusicObjects = Resources.FindObjectsOfTypeAll(typeof(BackgroundMusic)) as BackgroundMusic[];
        foreach(var bg in backgroundmusicObjects)
        {
            bg.GetComponent<BackgroundMusic>().SetVolume();
        }
    }

    void SetSFXSound()
    {
        var sfxObjects = Resources.FindObjectsOfTypeAll(typeof(ActorSoundManager)) as ActorSoundManager[];
        foreach(var sfxSource in sfxObjects)
        {
            sfxSource.GetComponent<ActorSoundManager>().SetClipVolume();
        }
    }

    void SetAmbientNoise()
    {
        var ambientObjects = Resources.FindObjectsOfTypeAll(typeof(AmbientNoise)) as AmbientNoise[];
        foreach(var ambientObject in ambientObjects)
        {
            ambientObject.GetComponent<AmbientNoise>().SetVolume();
        }
    }
}
