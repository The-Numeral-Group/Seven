﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; 

public class Corruption_PostProcessing : MonoBehaviour
{
    public Volume postProcessingVolume;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.gameObject.CompareTag("Player"))
        {
            return;
        }
        postProcessingVolume.weight = 1;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.gameObject.CompareTag("Player"))
        {
            return;
        }
        postProcessingVolume.weight = 0;
    }

}
