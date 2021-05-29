using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoSpotlight : ActorAbilityCoroutine<int>
{
    //FIELDS---------------------------------------------------------------------------------------
    [Tooltip("The mesh in which the spotlight will randomly appear in.")]
    public GameObject lightMesh;

    [Tooltip("The light prefab to instantiate.")]
    public GameObject lightPrefab;

    [Tooltip("How much time the user must spend in the light before recieving its effects.")]
    public float lightTime;

    [Tooltip("[DEBUG] How long the user should be stuck in place when they grab the spotlight")]
    public float debugDelay = 0.25f;

    //an internal reference to the currently active spotlight 
    private GameObject spotlight;

    //reference to the lightMesh's literal mesh for ease
    private Bounds lMesh;

    //internal timer for measuring how long the user has been in the light
    private float lTimer = 0f;

    //internal flag for whether or noth the user of this ability obtained the spotlight
    private bool userObtained = false;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        //get lightMesh's mesh
        lMesh = lightMesh.GetComponent<Collider2D>().bounds;
    }

    protected override IEnumerator InternCoroutine(params int[] args)
    {
        //reset obtainment flag
        userObtained = false;

        //represents the area of the lMesh

        //Pick a random point for the light to appear in
        var randomDestinationVec = new Vector3(
            Random.Range(-1f, 1f) * lMesh.size.x,
            Random.Range(-1f, 1f) * lMesh.size.y,
            0f
        );

        //create a light at that point
        spotlight = Instantiate(lightPrefab, randomDestinationVec, Quaternion.identity);

        //fun visual effects will go here

        //set the user to move towards the spotlight
        user.myMovement.MoveActor(
            (spotlight.transform.position - user.gameObject.transform.position).normalized
        );

        //while the spotlight lays unclaimed, keep the ability going
        //yield return new WaitWhile( () => spotlight );
        while(spotlight)
        {
            //set the user to move towards the spotlight
            user.myMovement.MoveActor(
                (spotlight.transform.position - user.gameObject.transform.position).normalized
            );

            yield return null;
        }

        //give the user a moment to think
        yield return null;

        Debug.Log($"user obtained: {userObtained}");

        /*//if the user got the spotlight AND they have Ego1's animator...
        if(userObtained && user.myAnimationHandler is Ego1AnimationHandler)
        {
            //let the user do a flex animation before the ability ends
            
        }*/

        if(!userObtained) yield break;

        ///DEBUG
        //normally you'd want to make the user wait for the animation to finish,
        //but that just isn't working right now
        user.myMovement.MoveActor(Vector2.zero);
        var flex = user.myAnimationHandler.TrySetTrigger("ego_flex");
        yield return new WaitForSeconds(debugDelay);
        ///DEBUG

        Debug.Log("spotlight finished");
    }

    /*an additional collision detector that ignores everything that isn't the spotlight (or 
    everything if the ability isn't active). When it hits the mesh, it will decrement
    lTimer by Time.deltaTime. if lTimer hits 0, the message OnInteract (user) will be passed to
    interact with, and thus claim, the spotlight*/
    /*void OnCollisionEnter2D(Collision2D collided)
    {
        if (this.isFinished || collided.gameObject != spotlight) {return;}

        Debug.Log("EgoSpotlight: user in light!");

        //increment timer
        lTimer += Time.deltaTime;

        //check time
        if(lTimer >= lightTime)
        {
            lTimer = 0f;

            //We use send message for consistency, that's how OnInteract is designed to be used
            //If the message has no reciever, an error will be throw.
            collided.gameObject.SendMessage
            (
                "OnInteract",
                user
            );
        }
    }*/

    /*an additional collision detector that ignores everything that isn't the spotlight (or 
    everything if the ability isn't active). When it hits the mesh, it will decrement
    lTimer by Time.deltaTime. if lTimer hits 0, the message OnInteract (user) will be passed to
    interact with, and thus claim, the spotlight*/
    void OnTriggerStay2D(Collider2D collided)
    {
        if (this.isFinished || collided.gameObject != spotlight) {return;}

        Debug.Log("EgoSpotlight: user in light!");

        //increment timer
        lTimer += Time.deltaTime;

        //check time
        if(lTimer >= lightTime)
        {
            lTimer = 0f;

            userObtained = true;

            //We use send message for consistency, that's how OnInteract is designed to be used
            //If the message has no reciever, an error will be throw.
            collided.gameObject.SendMessage
            (
                "OnAnyInteract",
                user
            );
        }
    }
}
