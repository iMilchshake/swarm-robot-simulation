using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RobotBehaviour
{
    public RobotScript parent;

    protected RobotBehaviour(RobotScript parent)
    {
        this.parent = parent;
    }


    public abstract void RecieveMessage<T>(Message<T> m);
    public abstract void DoStep();
}
