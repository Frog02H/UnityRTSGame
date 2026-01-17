using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class StateBase : IState
{
    public Actor actor;

    public StateBase(Actor actor)
    {
        this.actor = actor;
    }

    public virtual void State_Enter()
    {

    }

    public virtual void State_Exit()
    {
        
    }

    public virtual void State_HandleInput()
    {

    }

    public virtual void State_Update()
    {

    }

}
