using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private IState currentState;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentState.State_Update();
        currentState.State_HandleInput();
    }

    public void ChangeStateTo(IState newState)
    {
        currentState.State_Exit();
        currentState = newState;
        currentState.State_Enter();
    }
}
