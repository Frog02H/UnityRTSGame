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
        
        public State_Idle() : base() { Debug.Log("State_Idle 构造函数被调用！"); }

        public override void State_Enter(Actor actor)
        {
            // actor.animator.Play("Idle");
            actor.tai.isIdle = true;
            // 现在暂时没用bool控制动画机，所以这个bool值设置现在也没啥用
            // actor.animator.SetBool("isIdle", actor.tai.isIdle);
            actor.StayHere();
            actor.tai.theAction = TheActionIs.TheAction.Idle;
            Debug.Log("State_Idle 的 State_Enter！");
        }

        public override void State_Exit(Actor actor)
        {
            actor.tai.isIdle = false;
            // 现在暂时没用bool控制动画机，所以这个bool值设置现在也没啥用
            // actor.animator.SetBool("isIdle", actor.tai.isIdle);
            actor.tai.theAction = TheActionIs.TheAction.none;
            Debug.Log("State_Idle 的 State_Exit！");
        }

        public override void State_HandleInput(Actor actor)
        {
            
        }

        public override void State_Update(Actor actor)
        {

        }

    }
}