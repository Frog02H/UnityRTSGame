using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
            currentState = StateFactory.instance.GetState<State_Idle>(actor);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
