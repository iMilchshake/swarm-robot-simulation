using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.PlayerLoop;

// this behaviour should only be used with n=1
// the robot will walk towards the goal
// in a straight line to determine the fastest possible time

public class RobotBehaviourSoloCheating : RobotBehaviour
{
    public RobotBehaviourSoloCheating(RobotScript robot) : base(robot) { }

    public override void DoStep()
    {
        if (!robot.moving)
        {
            robot.SetTargetLocation(ControllerScript.ctrlScript.goal.transform.position);
        }
    }

    public override void RecieveMessage<T>(Message<T> m, RobotScript sender)
    {
    }
}
