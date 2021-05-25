using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrathShockwave : ActorAbilityFunction<Actor, int>
{
    public string animTrigger;
    public override void Invoke(ref Actor user)
    {
        this.user = user;
        if (usable)
        {
            isFinished = false;
            InternInvoke(user);
        }
    }
    protected override int InternInvoke(params Actor[] args)
    {
        // Making sure the movementDirection and dragDirection have been resetted.
        user.myMovement.MoveActor(Vector2.zero);
        user.myMovement.DragActor(Vector2.zero);

        if (animTrigger.Length != 0)
        {
            user.myAnimationHandler.TrySetTrigger(animTrigger);
        }

        // Chooses type of shockwave
        int shockwaveType = (int)Random.Range(0, 2);
        if(shockwaveType == 0)
        {
            // 4 Shockwave Pillars
        }
        else
        {
            // 1 Full Room Shockwave
        }
        // Temporary calling end function
        StartCoroutine(CheckIfAnimFinished());

        return 0;
    }
    // Temporary calling end function
    private IEnumerator CheckIfAnimFinished()
    {
        while (this.user.myAnimationHandler.IsInState(animTrigger))
        {
            yield return new WaitForFixedUpdate();
        }
        FinishShockwave();
    }

    // Temporary calling end function
    private void FinishShockwave()
    {
        this.isFinished = true;
    }
}
