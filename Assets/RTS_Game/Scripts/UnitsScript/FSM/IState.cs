using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IState
{
    public void State_Enter(Actor actor);
    public void State_Update(Actor actor);
    public void State_Exit(Actor actor);
    public void State_HandleInput(Actor actor);
}
