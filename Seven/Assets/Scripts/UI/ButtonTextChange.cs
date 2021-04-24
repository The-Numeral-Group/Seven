using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextChange : MonoBehaviour
{
    public Text text;

    public GameObject GameSaveManager;

    public float switch_duration;
    
    // Start is called before the first frame update
    void Start()
    {
        this.text = this.gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void textChangePerm(string newText)
    {
        text.text = newText;
    }

    public void textChangeTemp(string newText)
    {
        StartCoroutine(changeTextDuration(newText));
    }

    private IEnumerator changeTextDuration(string newText)
    {
        string prevText = text.text;

        text.text = newText;
        yield return new WaitForSeconds(this.switch_duration);
        text.text = prevText;
    }

    public void checkSaveFileExists()
    {
        if (GameSaveManager.GetComponent<GameSaveManager>().getBoolValue(0))
        {
            this.textChangeTemp("No Save File!");
        }
    }
}
