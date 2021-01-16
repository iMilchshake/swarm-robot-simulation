
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RobotBehaviour
{
    public RobotScript robot;

    protected RobotBehaviour(RobotScript robot)
    {
        this.robot = robot;
    }


    public abstract void RecieveMessage<T>(Message<T> m, RobotScript sender);
    public abstract void DoStep();
}
