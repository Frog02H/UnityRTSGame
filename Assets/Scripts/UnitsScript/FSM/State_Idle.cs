using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class State_Idle : StateBase
{
    public State_Idle(Actor actor) : base(actor) {}

    public override void State_Enter()
    {
        // actor.animator.Play("Idle");
        // actor.
        actor.animator.SetBool("isIdle", true);
        actor.StayHere();
    }

    public override void State_Exit()
    {
        actor.animator.SetBool("isIdle", false);
    }

    public override void State_HandleInput()
    {
        if (actor.currentTask != null)
        {
            
        }
    }

    public override void State_Update()
    {

    }

}
