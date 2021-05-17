using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityInitiator : ActorAbilityInitiator
{
    /*these are the two abilities that the player sees on their UI.*/
    /*public ActorAbility selectedAbilityAlpha;
    public ActorAbility selectedAbilityBeta;*/
    public ActorAbility selectedAbility;

    /*This is the player's implicit dodge. Switch this out
    to change how the player dodges*/
    public ActorAbility playerDodge;

    /*This is the player's implicit attaack. Switch this out
    to change how the player attacks*/
    public ActorAbility playerAttack;

    /*This is the player's implicit interact. Switch this out
    to change how the player interacts*/
    public ActorAbility playerInteract;

    //internal VFX manager
    private PlayerAbilityVFX VFXManager;

    // If player can dodge or not. 
    public bool canDodge { get; set; }

    void Awake()
    {
        //Manual initialization, 'cause I (Thomas) have come to realize it can't be done automatically

        /*this.abilities.Add("Player" + nameof(selectedAbilityAlpha), selectedAbilityAlpha);
        AbilityRegister.PLAYER_SELECTED_A = "Player" + nameof(selectedAbilityAlpha);

        this.abilities.Add("Player" + nameof(selectedAbilityBeta), selectedAbilityBeta);
        AbilityRegister.PLAYER_SELECTED_B = "Player" + nameof(selectedAbilityBeta);*/

        this.abilities.Add("" + nameof(playerAttack), playerAttack);
        AbilityRegister.PLAYER_ATTACK = "" + nameof(playerAttack);

        this.abilities.Add("" + nameof(playerDodge), playerDodge);
        AbilityRegister.PLAYER_DODGE = "" + nameof(playerDodge);

        this.abilities.Add("" + nameof(playerInteract), playerInteract);
        AbilityRegister.PLAYER_INTERACT = "" + nameof(playerInteract);

        this.canDodge = true;

        //also construct a VFX manager
        //VFXManager = new PlayerAbilityVFX(this);
    }

    //Don't know if this is needed, but just using the player actor to pass by ref to the invoke for attack and dodge.
    //It is, and every actor probably needs it since each ability needs their user. I've added it to
    //  ActorAbilityInitiator, so now it's innate to the class -Thomas
    //public Actor playerActor;

    /* Update is called once per frame
    void Update()
    {
        
    }*/

    //this is the method called by an input press
    public void OnAttack()
    {
        DoAttack();
    }

    public override void DoAttack()
    {
        playerAttack.Invoke(ref userActor);
    }

    public void OnDodge()
    {
        DoDodge();
    }

    public void DoDodge()
    {
        if(canDodge)
        {
            playerDodge.Invoke(ref userActor);
        }
    }

    public void OnInteract()
    {
        DoInteract();
    }

    public void DoInteract()
    {
        playerInteract.Invoke(ref userActor);
    }

    void OnAbility()
    {
        //Since the the cooldown for an ability is tied to actorabilityfunction
        if (selectedAbility != null && selectedAbility.getUsable() && selectedAbility.getIsFinished())
        {
            selectedAbility.Invoke(ref userActor);
            MenuManager.ABILITY_MENU.PutButtonOnCooldown(selectedAbility.getCooldown(), selectedAbility);
        }
    }

    void OnNavigateLeftAbility()
    {
        if (selectedAbility == null || selectedAbility.getIsFinished())
        {
            MenuManager.ABILITY_MENU.SelectLeftAbility();
        }
    }

    void OnNavigateRightAbility()
    {
        if (selectedAbility == null || selectedAbility.getIsFinished())
        {
            MenuManager.ABILITY_MENU.SelectRightAbility();
        }
    }

    //private class used for messing with the player's material on ability activation
    private class PlayerAbilityVFX
    {
        //FIELDS-----------------------------------------------------------------------------------
        //the current still-happening abilities, according to DoAbilityMaterial
        //this list will be used as a queue internally
        private List<ActorAbility> currentAbilities;

        //the current material timer, if any. Might be null.
        private Coroutine currentAbilityTimer;

        //the player's spriteRender at construction time
        private SpriteRenderer renderer;

        //the Player's AbilityInitiator to run the Coroutine on
        private PlayerAbilityInitiator PAI;

        //the special material to put on the player
        private Material specialMat;

        //the player's material the last time MaterialSwap was started
        private Material originalMat;

        //CONSTRUCTORS-----------------------------------------------------------------------------
        public PlayerAbilityVFX(PlayerAbilityInitiator PAI, Material specialMat)
        {
            this.PAI = PAI;
            this.renderer = PAI.gameObject.GetComponent<SpriteRenderer>();
            this.specialMat = specialMat;

            currentAbilities = new List<ActorAbility>();
        }

        //METHODS----------------------------------------------------------------------------------
        /*saves the passed ability as the lastAbilityUsed, then starts a new MaterialSwap, if one
        isn't active.*/
        public void DoAbilityMaterial(ActorAbility ability)
        {
            //add this ability to the queue
            currentAbilities.Add(ability);

            //if a timer isn't currently going, make it go!
            if(currentAbilityTimer == null) {PAI.StartCoroutine(MaterialSwap());}
        }

        /*Swaps out the player's material for something else as long as the last ability used isn't
        finished. It should still*/
        IEnumerator MaterialSwap()
        {
            //swap the materials
            originalMat = renderer.material;
            renderer.material = specialMat;

            yield return new WaitUntil( () => 
            {
                //until the queue is empty...
                while(currentAbilities.Count != 0)
                {
                    //check if the head is finished
                    if(currentAbilities[0].getIsFinished())
                    {
                        //if so, remove it
                        currentAbilities.RemoveAt(0);
                    }
                    else
                    {
                        //if not, return false. It won't be finished until at least the next frame
                        return false;
                    }
                }

                /*If the method has gotten here, that means that the queue as been emptied, and the
                player hasn't used any new abilities since the last time one was started. So, 
                the routine can stop waiting*/
                return true;
            } );

            //swap the materials back
            renderer.material = originalMat;

            //and clear the timer reference
            currentAbilityTimer = null;
        }


    }
}


