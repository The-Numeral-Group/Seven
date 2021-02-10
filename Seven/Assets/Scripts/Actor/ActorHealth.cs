using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*I just copied and modified the original ActorHealth.cs
I dropped the require component since ActorHealth doesn't need it
and having things just be able to take damage is useful. Also things
are floats now.*/

public class ActorHealth : MonoBehaviour
{
    //Public Fields (Inspector Accessable)
    public float startingMaxHealth = 100.0f;
    //this value determines how resistant to damage this thing is
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float damageResistance = 0.0f;

    //Public Properties (Publicly Accessable)
    public float maxHealth { get; set; }
    public float currentHealth { get; set; }
    public bool vulnerable { get; set; }

    private SpriteRenderer spriteRenderer;

    void Awake(){
        this.maxHealth = startingMaxHealth;
        this.currentHealth = this.maxHealth;
        this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }
    
    /*// Start is called before the first frame update
    We keep Start out of this in case subclasses need to use it
    void Start()
    {
            
    }*/

    public virtual void takeDamage(float damageTaken){
        if (!this.vulnerable)
        {
            return;
        }
        var damage = Mathf.Floor(damageTaken * (1.0f - damageResistance));
        Debug.Log("taking " + damage + " damage");

        //take the damage
        this.currentHealth -= damage;
        StartCoroutine(FlashRed());

        //if the attack killed the thing
        if(this.currentHealth <= 0){
            /*I'd like to use SendMessageOptions.RequireReciever to make it so
            that the game vomits if we try to kill something that cannot die,
            but I just don't know how*/
            this.gameObject.SendMessage("DoActorDeath");//, null, RequireReciever);


        }
    }

    // A visual indicator if actor has received damage.
    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = Color.white;
    }
}