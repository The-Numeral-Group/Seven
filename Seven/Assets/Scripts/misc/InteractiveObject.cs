using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractiveObject : MonoBehaviour
{
    public void Invoke()
    {
        SceneManager.LoadScene("Tutorial_Cutscene0");
    }
}
