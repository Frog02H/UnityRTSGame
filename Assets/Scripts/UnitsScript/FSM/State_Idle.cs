using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_Idle : StateBase
{
    // public State_Idle(Actor actor) : base(actor) {}

    public override void State_Enter(Actor actor)
    {
        // actor.animator.Play("Idle");
        actor.tai.isIdle = true;
        actor.animator.SetBool("isIdle", true);
        actor.StayHere();
    }

    public override void State_Exit(Actor actor)
    {
        actor.tai.isIdle = false;
        actor.animator.SetBool("isIdle", false);
    }

    public override void State_HandleInput(Actor actor)
    {
        
    }

    public override void State_Update(Actor actor)
    {

    }

}
