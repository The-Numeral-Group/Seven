public static class AbilityRegister
{
    /*
    HEY LOOK AT ME READ ME YA DUMMY

    This is the Ability Register! It's a pain
    to try and keep track of a bunch of magic strings
    inside of various scripts to access abilities inside of
    ActorAbilityInitiators and other scripts. So, this space
    exists to define an area to place abilities that we want to
    be publically accessible.

    For example, the player's dodge will be key'd under whatever
    is listed as the key string of the player's dodge in PlayerAbilityInitiator.
    However, AbilityRegister.PLAYER_DODGE will always be AbilityRegister.PLAYER_DODGE.
    So, if we save the key to player's dodge to AbilityRegister.PLAYER_DODGE, then
    any script can use AbilityRegister to look up an ability! If a script tries to access
    a value that doesn't exist in AbilityRegister, the compiler will simply tell it to
    fuck off! No more magic number strings!

    Anyways, to add stuff to the register, just add a public static string variable like the ones below.
    You should always use this registry when accessing an ability outside of an Actor's home strcuture 
    (i.e. abilities that belong to other actors).
    */

    //PLAYER---------------------------------------------------------------------------------------
    public static string PLAYER_ATTACK      = "";
    public static string PLAYER_DODGE       = "";
    public static string PLAYER_INTERACT    = "";
    public static string PLAYER_SELECTED_A  = "";
    public static string PLAYER_SELECTED_B  = "";

    //GLUTTONY-------------------------------------------------------------------------------------
    public static string GLUTTONY_BITE          = "";
    public static string GLUTTONY_CRUSH         = "";
    public static string GLUTTONY_PHASEZERO_SPECIAL = "";
    public static string GLUTTONY_PROJECTILE    = ""; 
    public static string GLUTTONY_PHASETWO_SPECIAL = "";
    public static string GLUTTONY_PHASETWO_PROJECTILE = "";  

    //PRIDE----------------------------------------------------------------------------------------
    public static string PRIDE_CLOSE_ATTACK = "";
    public static string PRIDE_FAR_ATTACK   = "";
    public static string PRIDE_SPECIAL      = "";

    //GHOST KNIGHT---------------------------------------------------------------------------------
    public static string GHOSTKNIGHT_SLASH = "";
    public static string GHOSTKNIGHT_PROJECTILE = "";
    public static string GHOSTKNIGHT_SPECIAL = "";

    //SLOTH----------------------------------------------------------------------------------------
    public static string SLOTH_PHYSICAL = "";
    public static string SLOTH_RANGE = "";
    public static string SLOTH_SPECIAL = "";

    //EGO PHASE 2----------------------------------------------------------------------------------
    public static string EGO_BEAM = "";
    public static string EGO_WALL = "";
    public static string EGO_SUMMON = "";

    //INDULGENCE ----------------------------------------------------------------------------------
    public static string INDULGENCE_CRUSH = "";
    public static string INDULGENCE_WALLCRAWL = "";
    public static string INDULGENCE_PHYSICAL = "";
    public static string INDULGENCE_PROJECTILE = "";
}
