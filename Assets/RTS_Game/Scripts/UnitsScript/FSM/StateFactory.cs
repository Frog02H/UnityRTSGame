using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFactory : MonoBehaviour
{
    public static StateFactory instance;
    private StateFactory() { }

    // private Dictionary<Type, StateBase> statesPool = new();
    private Dictionary<int, Dictionary<Type, IState>> statesPool = new();

    public IState GetState<T>(Actor actor) where T : IState, new()
    {
        if (!statesPool.ContainsKey(actor.ActorID))
        {
            statesPool[actor.ActorID] = new Dictionary<Type, IState>();
        }

        Dictionary<Type, IState> states = statesPool[actor.ActorID];

        Type type = typeof(T);

        if (!states.ContainsKey(type))
        {
            states[type] = new T();
        }

        return states[type];
    }

    public void ClearPool()
    {
        statesPool.Clear();
    }

    /* 
    public StateBase GetState<T>(Actor actor) where T : StateBase, new()
    {
        Type type = typeof(T);

        if (!statesPool.ContainsKey(type))
        {
            statesPool[type] = new T();
        }

        statesPool[type].actor = actor;

        return statesPool[type];
    } 
    */
}
