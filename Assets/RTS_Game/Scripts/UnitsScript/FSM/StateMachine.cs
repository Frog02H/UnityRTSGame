using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RTS_Game.Units
{
    public class StateMachine : MonoBehaviour
    {
        private Actor actor;
        private IState currentState;

        // 初始化引用，供给Actor自行调用
        public void LanchMachine(Actor actor)
        {
            this.actor = actor;

            if (currentState == null)
            {
                if (StateFactory.instance == null)
                {
                    Debug.Log("You are Right! StateFactory.instance 是 NULL!");
                }
                else
                {
                    Debug.Log("StateFactory.instance is NOT NULL!");
                }

                if (actor != null)
                {
                    // State_Idle state_Idle = new State_Idle();

                    /* 
                    if(StateFactory.instance.TestState<State_Idle>(actor) == null)
                    {
                        Debug.Log("TestState去掉约束的，成功了！");
                    }
                    else
                    {
                        Debug.Log("TestState去掉约束的，大失败！");
                    }
                    */

                    /*
                    if (StateFactory.instance.GetState<State_Idle>(actor) == null)
                    {
                        Debug.Log("GetState返回值是null啊！");
                    }
                    */

                    // currentState = StateFactory.instance.GetState<State_Idle>(actor);
                    
                    /*
                    if (currentState == null)
                    {
                        Debug.Log("GetState返回值是null啊！");
                    }
                    */
                }
                else
                {
                    Debug.Log("actor为null!");
                }
            }
        }

        void Awake()
        {
            
        }
        // Start is called before the first frame update
        void Start()
        {
            LanchMachine(GetComponent<Actor>());

            if (StateFactory.instance != null)
            {
                Debug.Log("StateFactory里Start的StateFactory.instance != null");
                if (currentState == null)
                {
                    currentState = StateFactory.instance.GetState<State_Idle>(actor);
                }
            }
            else
            {
                Debug.Log("StateFactory.instance没有初始化");
            }
        }

        // Update is called once per frame
        void Update()
        {
            currentState.State_Update(actor);
            currentState.State_HandleInput(actor);
        }

        public void ChangeStateTo(IState newState)
        {
            if (newState == null)
            {
                Debug.LogError(actor.gameObject.name + "的状态机尝试切换成空状态。");
                return;
            }
            currentState.State_Exit(actor);
            currentState = newState;
            currentState.State_Enter(actor);
        }
    }
}