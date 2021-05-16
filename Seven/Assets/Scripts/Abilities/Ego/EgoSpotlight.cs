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

    //an internal reference to the currently active spotlight 
    private GameObject spotlight;

    //reference to the lightMesh's literal mesh for ease
    private Mesh lMesh;

    //internal timer for measuring how long the user has been in the light
    private float lTimer = 0f;

    //METHODS--------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        //get lightMesh's mesh
        lMesh = lightMesh.GetComponent<MeshFilter>().mesh;
    }

    protected override IEnumerator InternCoroutine(params int[] args)
    {
        //represents the area of the lMesh
        Bounds meshBound = lMesh.bounds;

        //Pick a random point for the light to appear in
        var randomDestinationVec = new Vector3(
            Random.Range(-1f, 1f) * meshBound.size.x,
            Random.Range(-1f, 1f) * meshBound.size.y,
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

            //We use send message for consistency, that's how OnInteract is designed to be used
            //If the message has no reciever, an error will be throw.
            collided.gameObject.SendMessage
            (
                "OnAnyInteract",
                user
            );

            this.user.gameObject.SendMessage("animateFLEX", SendMessageOptions.DontRequireReceiver);
        }
    }
}
