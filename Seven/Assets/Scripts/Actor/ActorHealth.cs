using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*I just copied and modified the original ActorHealth.cs
I dropped the require component since ActorHealth doesn't need it
and having things just be able to take damage is useful. Also things
are floats now.*/

public class ActorHealth : MonoBehaviour
{
    //reference to the actor in charge of this component
    protected Actor hostActor;

    //Public Fields (Inspector Accessable)
    public float startingMaxHealth = 100.0f;
    //this value determines how resistant to damage this thing is
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float damageResistance = 0.0f;
    //How long the actor stays invulnerable for from taking a hit
    [Range(0, 256)]
    public int invincibilityDuration = 1;

    public bool startInvulnerable = false;

    //Public Properties (Publicly Accessable)
    //The actors maximum health
    public float maxHealth { get; set; }
    //The actors current health
    public float currentHealth { get; set; }
    //The following variables will be used to handle invulnerability
    //Whether or not the actor can be hit.
    [SerializeField]
    public bool vulnerable { get; protected set; }
    //The amount of time remaining on the actors invulnerability.
    float timeInvulnerable;
    //Ppointers to coroutines for stopping and starting.
    IEnumerator MakeVulnerablePointer;
    IEnumerator FlashRedPtr;
    IEnumerator BlinkPtr;

    //Reference to the objects sprite renderer
    SpriteRenderer spriteRenderer;
    Color modifiedColor;
    Color defaultColor;

    void Awake(){
        hostActor = this.GetComponent<Actor>();
        this.maxHealth = startingMaxHealth;
        this.currentHealth = this.maxHealth;
        this.vulnerable = !startInvulnerable;
        timeInvulnerable = 0f;
        MakeVulnerablePointer = ExtendInvulnerability(invincibilityDuration, false);
        FlashRedPtr = FlashRed();
        BlinkPtr = BlinkEffect();
        this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }
    
    /*// Start is called before the first frame update
    We keep Start out of this in case subclasses need to use it
    void Start()
    {
            
    }*/

    public virtual void takeDamage(float damageTaken, bool bypassDamageResistance=false){
        Debug.Log(this.gameObject.name + " should take damage");
        if (!this.vulnerable)
        {
            return;
        }

        float damage;
        if(bypassDamageResistance)
        {
            damage = damageTaken;
        }
        else
        {
            damage = Mathf.Lerp(damageTaken, 0, damageResistance);
        }
        //var damage = Mathf.Floor(damageTaken * (1.0f - damageResistance));
        Debug.Log(this.gameObject.name + " taking " + damage + " damage");

        //take the damage
        this.currentHealth -= damage;
        //StartCoroutine(FlashRed());
        //StartCoroutine(MakeInvulnerableAfterDamage());

        //trigger actor damage effects
        this.gameObject.BroadcastMessage(
            "DoActorDamageEffect", 
            damage, 
            SendMessageOptions.DontRequireReceiver
        );

        //if the attack killed the thing
        if(this.currentHealth <= 0){
            /*I'd like to use SendMessageOptions.RequireReciever to make it so
            that the game vomits if we try to kill something that cannot die,
            but I just don't know how*/
            this.gameObject.SendMessage("DoActorDeath");//, null, RequireReciever);


        }
    }

    /*Used to keep track of a players accumulated invulnerability time. This will allow us to
    have multiple calls add to an objects invulnerability time.*/
    IEnumerator ExtendInvulnerability(float duration, bool value)
    {
        timeInvulnerable = duration;
        for (float i = 0; i < duration; i += 0.1f)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            timeInvulnerable -= 0.1f;
        }
        this.vulnerable = !value;
        timeInvulnerable = 0f;
    }

    /*Used to set the value of invulnerable, as well as for how long. 
    After the duration the objects vulnerability state will be set 
    to the opposite w/e boolean state was initially passed in. 
    E.g. if you pass in false, for duration seconds the object is invulnerable, 
    then it it wil become vulnerable.
    Pass in a negative value for duration for an object 
    to remain in that state with no duration to reset it*/
    public void SetVulnerable(bool value, float duration, bool accumulateInvincibility = true)
    {
        this.vulnerable = value;
        StopCoroutine(MakeVulnerablePointer);
        if(duration < 0)
        {
            timeInvulnerable = 0f;
        }
        else
        {
            if (accumulateInvincibility)
            {
                duration += timeInvulnerable;
            }
            MakeVulnerablePointer = ExtendInvulnerability(duration, value);
            StartCoroutine(MakeVulnerablePointer);
        }
    }

    //unrelated: invincibility fix is tied to rigidbody settings
    //https://answers.unity.com/questions/1208412/ontriggerstay2dcollider2d-coll-does-not-detect-a-c.html
    public void DefaultDamageEffect()
    {
        StopCoroutine(FlashRedPtr);
        StopCoroutine(BlinkPtr);
        spriteRenderer.color = defaultColor;
        modifiedColor = defaultColor;
        if (invincibilityDuration > 0)
        {
            SetVulnerable(false, invincibilityDuration, false);
            BlinkPtr = BlinkEffect();
            StartCoroutine(BlinkPtr);
        }
        FlashRedPtr = FlashRed();
        StartCoroutine(FlashRedPtr);
    }

    // A visual indicator if actor has received damage.
    IEnumerator FlashRed()
    {
        modifiedColor.r = 1;
        modifiedColor.g = 0;
        modifiedColor.b = 0;
        spriteRenderer.color = modifiedColor;
        yield return new WaitForSeconds(0.3f);
        modifiedColor.r = defaultColor.r;
        modifiedColor.g = defaultColor.g;
        modifiedColor.b = defaultColor.b;
        spriteRenderer.color = modifiedColor;
    }

    IEnumerator BlinkEffect()
    {
        float blinkSpeed = invincibilityDuration > 0.2f * 2f ? 0.2f : invincibilityDuration / 2f;
        for (float i = 0; i < invincibilityDuration; i += blinkSpeed * 2f)
        {
            yield return new WaitForSeconds(blinkSpeed);
            modifiedColor.a = 0;
            spriteRenderer.color = modifiedColor;
            yield return new WaitForSeconds(blinkSpeed);
            modifiedColor.a = defaultColor.a;
            spriteRenderer.color = modifiedColor;
        }
    }
}