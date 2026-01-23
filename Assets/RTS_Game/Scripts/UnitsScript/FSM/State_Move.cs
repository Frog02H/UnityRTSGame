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
            // 现在暂时没用bool控制动画机，所以这个bool值设置现在也没啥用
            // actor.animator.SetBool("isMove", actor.tai.isMove);
            actor.tai.theAction = TheActionIs.TheAction.Move;
            actor.SetDestination(actor.tpi.target);

            actor.actorAction.onFinMove += actor.actorAction.checkMoveStop;

            Debug.Log("State_Move 的 State_Enter！");
        }

        public override void State_Exit(Actor actor)
        {
            actor.tai.isMove = false;
            // 现在暂时没用bool控制动画机，所以这个bool值设置现在也没啥用
            // actor.animator.SetBool("isMove", actor.tai.isMove);
            actor.tai.theAction = TheActionIs.TheAction.none;
            Debug.Log("State_Move 的 State_Exit！");
        }

        public override void State_HandleInput(Actor actor)
        {

        }

        public override void State_Update(Actor actor)
        {
            actor.actorAction.onFinMove?.Invoke(actor);
        }

    }
}