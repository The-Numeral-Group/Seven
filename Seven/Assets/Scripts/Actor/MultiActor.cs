using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiActor : MonoBehaviour
{
    /*The list of every "phase" (that is, unique Actor objects)
    that this MultiActor uses. They are ordered by index, and will
    be traversed by index unless manually jumped around.
    
    Actually just make people start them as transform children idk.*/
    //[Tooltip("The list of every \"phase\" (that is, unique Actor objects) that this MultiActor uses. They are ordered by index, and will be traversed by index unless manually jumped around.")]
    //public List<GameObject> actorPhases;
    public List<GameObject> actorPhases{ get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        var myTransform = this.gameObject.transform;

        this.actorPhases = new List<GameObject>();

        //Step 0: If this object has an Actor parent, assume that parent is Phase 0
        if(myTransform.parent != null && myTransform.parent.gameObject.GetComponent<Actor>() != null)
        {
            this.actorPhases.Add(myTransform.parent.gameObject);

            //Step 0.5: give Phase 0 a MATC
            myTransform.parent.gameObject.AddComponent<MultiActorTransitionController>().hostMultiActor = this;
        }

        //Step 1: Add every phase to the actorPhases list
        for(int i = 0; i < myTransform.childCount; ++i)
        {
            var addedChild = myTransform.GetChild(i).gameObject;
            if(addedChild.GetComponent<Actor>() != null)
            {
                this.actorPhases.Add(addedChild);

                //Step 1.3: give this phase a MATC
                addedChild.AddComponent<MultiActorTransitionController>().hostMultiActor = this;

                //Step 1.6: make sure this phase isn't on right now
                addedChild.SetActive(false);
            }
        }

        /*Step 2: If we're not already at Phase 0 (i.e. if the MultiActor's parent
        isn't an actor or doesn't exist), go to Phase 0*/
        if(this.gameObject.transform.parent == null){GoToPhase(0);}
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public Actor GoToPhase(int index)
    {
        Transform myTransform = this.gameObject.transform;
        Transform oldPhase = myTransform.parent;

        /*we don't know if the index actually exists, ?[] 
        will make newPhase null if it doesn't. The ?. before
        transform has a similar effect*/
        //also screw you more ternaries
        Transform newPhase;
        if(index < this.actorPhases.Count)
        {
            newPhase = actorPhases?[index]?.transform;
        }
        else
        {
            return null;
        }

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

        //Check if newPhase actually exists. If it doesn, we can
        //just stop here and return null.
        if(newPhase != null)
        {
            //de-child the new phase. It's now a free object
            newPhase.parent = null;

            //make the MultiActor a child of newPhase. newPhase has now
            //  replaced oldPhase
            myTransform.parent = newPhase;

            //activate newPhase. It is now the commanding gameObject.
            //  Because phases are instantiated inactive, Awake will be
            //  called at this time for every script on newPhase.
            newPhase.gameObject.SetActive(true);

            return newPhase.gameObject.GetComponent<Actor>();
        }
        else
        {
            return null;
        }

    }
}

public class MultiActorTransitionController : MonoBehaviour
{
    public MultiActor hostMultiActor = null;

    /*Tuples let us pair arbitrary data together really easy-like. This lets us use two
    seperate objects as an argument for a SendMessage, so we can pass an initialization function
    into our next phase!
    */
    public void NextPhase(Tuple<Actor, Action<Actor>> args)
    {
        var lastPhaseIndex = hostMultiActor.actorPhases.IndexOf(args.Item1.gameObject);

        var newPhase = hostMultiActor.GoToPhase(lastPhaseIndex + 1);

        if(newPhase != null)
        {
            /*This version of Invoke is Action.Invoke, not ActorAbility.Invoke
            Action is a subclass of the standard C# type Delegate*/
            if(args.Item2 != null){Debug.Log("we do have delegates");}
            else{Debug.Log("The delegates are null");}
            args.Item2?.Invoke(newPhase);
        }
        else
        {
            Debug.LogWarning("MultiActor attempted to shift to phase " + (lastPhaseIndex + 1) + ", which doesn't exist");
        }
    }

    public void ChangePhase(Tuple<int, Action<Actor>> args)
    {
        var newPhase = hostMultiActor.GoToPhase(args.Item1);

        if(newPhase != null)
        {
            /*This version of Invoke is Action.Invoke, not ActorAbility.Invoke
            Action is a subclass of the standard C# type Delegate*/
            args.Item2?.Invoke(newPhase);
        }
        else
        {
            Debug.LogWarning("MultiActor attempted to shift to phase " + args.Item1 + ", which doesn't exist");
        }
    }
}
