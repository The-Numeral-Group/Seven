using UnityEngine;
using UnityEngine.UI;

//Used with the settings sub menu within the pause menu
public class SettingsSubMenu : SubMenu
{
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider ambienceVolume;
    public Slider sfxVolume; 

    public void SetMasterVolume()
    {
        GameSettings.MASTER_VOLUME = masterVolume.value;
        Debug.Log(GameSettings.MASTER_VOLUME);
    }
    public void SetMusicVolume()
    {
        GameSettings.MUSIC_VOLUME = musicVolume.value;
    }
    public void SetAmbientVolume()
    {
        GameSettings.AMBIENT_VOLUME = ambienceVolume.value;
    }
    public void SetSFXVolume()
    {
        GameSettings.SFX_VOLUME = sfxVolume.value;
    }
}
