using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTS_Game.Units
{
    public class State_Move : StateBase
    {
        // public State_Move(Actor actor) : base(actor) {}

        public State_Move() : base() {}

        public override void State_Enter(Actor actor)
        {
            actor.tai.isMove = true;
            actor.animator.SetBool("isMove", actor.tai.isMove);
            actor.tai.theAction = TheActionIs.TheAction.Move;
            actor.SetDestination(actor.tpi.target);
        }

        public override void State_Exit(Actor actor)
        {
            actor.tai.isMove = false;
            actor.animator.SetBool("isMove", actor.tai.isMove);
            actor.tai.theAction = TheActionIs.TheAction.none;
        }

        public override void State_HandleInput(Actor actor)
        {

        }

        public override void State_Update(Actor actor)
        {

        }

    }
}