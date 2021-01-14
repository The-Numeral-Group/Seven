using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSwitchPhase : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //this.gameObject.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        StartCoroutine(SwitchInThreeSeconds());
    }

    private IEnumerator SwitchInThreeSeconds()
    {
        yield return new WaitForSeconds(3);

        var g = this.gameObject.GetComponent<Actor>();
        //var f = 
        if(Random.value < .5)
        {
            var x = (int)Random.Range(0.0f, 4.0f);
            Debug.Log("Going to " + x);
            this.gameObject.SendMessage("ChangePhase", new System.Tuple<int, System.Action<Actor>>(x, new System.Action<Actor>(RandomizeColor)));
        }
        else
        {
            Debug.Log("TIME TO EXPLODE");
            Destroy(this.gameObject);
        }
    }

    private void RandomizeColor(Actor actor)
    {
        actor.gameObject.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }
}
