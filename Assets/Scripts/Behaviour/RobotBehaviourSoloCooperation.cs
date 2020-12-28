using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.PlayerLoop;
using Random = System.Random;

// pseudocode
// step 1: walk randomly
// step 2: share goal position if known
// step 3: repeat

public class RobotBehaviourSoloCooperation : RobotBehaviour
{
    public RobotBehaviourSoloCooperation(RobotScript robot) : base(robot) { }

    public override void DoStep()
    {
        // step 1: walk randomly
        if (!robot.moving) // reached targetLocation
        {
            Vector2 pos = ControllerScript.RandomOuterPosition(5, 23);
            robot.SetTargetLocation(new Vector3(pos.x, 0, pos.y));
        }

        // step 2: share goal position if known
        if (robot.foundGoal)
        {
            robot.Broadcast(new Message<Vector3>(robot.goalPosition, 0)); // Share Goal Position
        }
    }

    public override void RecieveMessage<T>(Message<T> m, RobotScript sender)
    {
        if(m.GetData() is Vector3 vector)
        {
            if (!robot.foundGoal)
            {
                robot.GoalFound(vector);
            }
        }
    }
}
