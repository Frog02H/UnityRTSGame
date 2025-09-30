using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class State_Move : StateBase
{
    public State_Move(Actor actor) : base(actor) {}

    public override void State_Enter()
    {
        actor.animator.Play("Run");
        
    }

    public override void State_Exit()
    {
        
    }

    public override void State_HandleInput()
    {

    }

    public override void State_Update()
    {

    }

}
