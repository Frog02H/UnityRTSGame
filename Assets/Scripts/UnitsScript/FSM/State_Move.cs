using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_Move : StateBase
{
    // public State_Move(Actor actor) : base(actor) {}

    public override void State_Enter(Actor actor)
    {
        actor.tai.isMove = true;
        actor.animator.SetBool("isMove", true);
        actor.SetDestination(actor.tpi.target);
    }

    public override void State_Exit(Actor actor)
    {
        actor.tai.isMove = false;
        actor.animator.SetBool("isMove", false);
    }

    public override void State_HandleInput(Actor actor)
    {

    }

    public override void State_Update(Actor actor)
    {

    }

}
