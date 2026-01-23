using System.Collections;
using System.Collections.Generic;
using RTS_Game.Units;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace RTS_Game.Units
{
    public class ActorAction
    {
        public UnityAction<Actor> onFinMove;

        public void checkMoveStop(Actor actor)
        {
            if (!actor._agent.pathPending && actor._agent.remainingDistance <= actor._agent.stoppingDistance)
            {
                actor.SMSwiftTo(StateFactory.instance.GetState<State_Idle>(actor));
            }
        }
    }
}