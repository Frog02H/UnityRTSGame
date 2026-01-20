using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS_Game.Units
{
    public class StateFactory : MonoBehaviour
    {
        public static StateFactory instance {get; private set; }
        // private StateFactory() { }

        // private Dictionary<Type, StateBase> statesPool = new();
        private Dictionary<int, Dictionary<Type, IState>> statesPool = new();

        // Awake 是Unity内置的初始化方法
        private void Awake()
        {
            // 检查实例是否已存在
            if (instance != null && instance != this)
            {
                // 如果已存在另一个实例，则销毁这个新创建的实例，确保唯一性
                Destroy(this.gameObject);
            }
            else
            {
                // 如果不存在，将此实例设为单例
                instance = this;
                // 可选：使该游戏对象在加载新场景时不被销毁，用于全局管理器
                // DontDestroyOnLoad(this.gameObject);
            }
        }
        
        public IState GetState<T>(Actor actor) where T : IState, new()
        {
            Debug.Log("temp生成前！");
            State_Idle temp = new State_Idle();
            Debug.Log("temp已经生成完了！");
            if (temp != null)
            {
                Debug.Log("temp不为null！");
            }
            else
            {
                Debug.Log("temp是null！！！");
            }
            
            return temp;

            if (!statesPool.ContainsKey(actor.ActorID))
            {
                Debug.Log("statesPool[actor.ActorID]准备生成！");
                statesPool[actor.ActorID] = new Dictionary<Type, IState>();
            }

            Debug.Log("statesPool[actor.ActorID]已经存在！");
            Dictionary<Type, IState> states = statesPool[actor.ActorID];

            Type type = typeof(T);

            if (!states.ContainsKey(type))
            {
                Debug.Log("states[type]准备生成！");
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
}