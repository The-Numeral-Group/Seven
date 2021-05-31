using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientNoise : MonoBehaviour
{
    public List<AudioSource> sources;
    void Awake()
    {
        SetVolume();
    }
    // Start is called before the first frame update
    public void SetVolume()
    {
        foreach(AudioSource source in sources)
        {
            source.volume = GameSettings.MASTER_VOLUME * GameSettings.AMBIENT_VOLUME;
        }
    }
}
