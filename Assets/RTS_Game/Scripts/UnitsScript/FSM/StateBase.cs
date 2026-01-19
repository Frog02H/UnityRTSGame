using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace RTS_Game.Units
{
    public abstract class StateBase : IState
    {
        // public Actor actor;

        /*
        public StateBase(Actor actor)
        {
            this.actor = actor;
        }
        */

        public virtual void State_Enter(Actor actor)
        {

        }

        public virtual void State_Exit(Actor actor)
        {

        }

        public virtual void State_HandleInput(Actor actor)
        {
            /*
            // 不要用这个转换！
            // 如果通过枚举类型进入对应分支，却发现当前单位通用状态并没有更新，意味着根本
            switch(actor.tai.theAction)
            {
                case TheActionIs.TheAction.isIdle:
                    if(!actor.tai.isIdle)
                    {
                        actor.SMSwiftTo(StateFactory.instance.GetState<State_Idle>(actor));
                    }
                    break;
                case TheActionIs.TheAction.isMove:
                    if(!actor.tai.isMove)
                    {
                        actor.SMSwiftTo(StateFactory.instance.GetState<State_Move>(actor));
                    }
                    break;
                case TheActionIs.TheAction.isAttack:
                    if(!actor.tai.isAttack)
                    {
                        actor.SMSwiftTo(StateFactory.instance.GetState<State_Attack>(actor));
                    }
                    break;
                case TheActionIs.TheAction.isGuarding:
                    if(!actor.tai.isGuarding)
                    {
                        actor.SMSwiftTo(StateFactory.instance.GetState<State_Guard>(actor));
                    }
                    break;
                case TheActionIs.TheAction.isInVehicle:
                    if(!actor.tai.isInVehicle)
                    {
                        actor.SMSwiftTo(StateFactory.instance.GetState<State_InVehicle>(actor));
                    }
                    break;
                default:
                    break;
            } 
            */
        }

        public virtual void State_Update(Actor actor)
        {

        }
    }
}
