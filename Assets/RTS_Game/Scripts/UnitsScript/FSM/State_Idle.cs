using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace RTS_Game.Units
{
    public class State_Idle : StateBase
    {
        // public State_Idle(Actor actor) : base(actor) {}
        
        public State_Idle() : base() {}

        public override void State_Enter(Actor actor)
        {
            // actor.animator.Play("Idle");
            // actor.tai.isIdle = true;
            actor.tai.isIdle = true;
            // actor.animator.SetBool("isIdle", actor.tai.isIdle);
            actor.animator.SetBool("isIdle", actor.tai.isIdle);
            actor.StayHere();
            actor.tai.theAction = TheActionIs.TheAction.Idle;
        }

        public override void State_Exit(Actor actor)
        {
            actor.tai.isIdle = false;
            actor.animator.SetBool("isIdle", actor.tai.isIdle);
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