using UnityEngine;
﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.PlayerLoop;

public class RobotBehaviourSoloStupid : RobotBehaviour
{
    public RobotBehaviourSoloStupid(RobotScript robot) : base(robot) { }

    public override void DoStep()
    {
        if (robot.foundGoal)
        {
            robot.SetTargetLocation(robot.goalPosition);
            robot.Broadcast<Vector3>(new Message<Vector3>(robot.goalPosition, 0));
        }
        else if (!robot.moving)
        {
            robot.SetTargetLocation(new Vector3(UnityEngine.Random.Range(-22, 22), 0, UnityEngine.Random.Range(-22, 22)));
        }
    }

    public override void RecieveMessage<T>(Message<T> m, RobotScript sender)
    {
        if (m.GetData() is Vector3 vector)
        {
            if(robot.foundGoal == false)
            {
                robot.foundGoal = true;
                robot.goalPosition = vector;
                robot.SetTargetLocation(robot.goalPosition);
            }
        }
    }
}
