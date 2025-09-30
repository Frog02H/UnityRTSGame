using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFactory
{
    private Dictionary<Type, StateBase> statesPool = new();

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
}
