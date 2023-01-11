using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPlayerComponent : Icomponent
{
    protected GameObject player;

    public IPlayerComponent(GameObject player)
    {
        this.player = player;
    }

    public abstract void UpdateState(GameState state);
}