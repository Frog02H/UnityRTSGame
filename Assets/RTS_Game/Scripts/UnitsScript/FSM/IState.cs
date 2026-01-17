using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IState
{
    public void State_Enter();
    public void State_Update();
    public void State_Exit();
    public void State_HandleInput();
}
