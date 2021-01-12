using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiActor : MonoBehaviour
{
    /*The list of every "phase" (that is, unique Actor objects)
    that this MultiActor uses. They are ordered by index, and will
    be traversed by index unless manually jumped around.*/
    [Tooltip("The list of every \"phase\" (that is, unique Actor objects) that this MultiActor uses. They are ordered by index, and will be traversed by index unless manually jumped around.")]
    public List<GameObject> actorPhases;

    // Start is called before the first frame update
    void Start()
    {
        //Step 1: Instantiate every desired phase in an unactive state.
        //  Give them a MATC so they can easily change their phase
        foreach(GameObject actorPhase in actorPhases)
        {
            actorPhase.SetActive(false);
            var newPhase = Instantiate(actorPhase, this.gameObject.transform);
            var matc = newPhase.AddComponent<MultiActorTransitionController>();
            matc.hostMultiActor = this;
            /*
            we could do this line with...
            Instantiate(actorPhase, this.gameObject.transform).AddComponent<MultiActorTransitionController>().hostMultiActor = this;
            but that would be a massive pain
            */
        }

        //
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    /*Activates the next phase in the MultiActor's
    transform (that is, Child 0)*/
    public void NextPhase()
    {
        ChangePhase(0);
    }

    public void ChangePhase(int index)
    {
        var myTransform = this.gameObject.transform;
        var newPhase = myTransform.GetChild(index);
        var oldPhase = myTransform.parent;

        //Check if oldPhase actually exists. Some phases
        //might destroy themselves when they end.
        if(oldPhase != null)
        {
            //de-parent old phase
            myTransform.parent = null;

            //de-activate old phase
            oldPhase.gameObject.SetActive(false);

            //re-child old phase, in case we want it later
            oldPhase.parent = myTransform;
        }

        //de-child the new phase. It's now a free objecy
        newPhase.parent = null;

        //make the MultiActor a child of newPhase. newPhase has now
        //  replaced oldPhase
        myTransform.parent = newPhase;

        //activate newPhase. It is now the commanding gameObject.
        //  Because phases are instantiated inactive, Awake will be
        //  called at this time for every script on newPhase.
        newPhase.gameObject.SetActive(true);

    }
}

public class MultiActorTransitionController : MonoBehaviour
{
    public MultiActor hostMultiActor;
}
