using UnityEngine;
﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.PlayerLoop;


//0:	set walking pos(-> random position)
//1:    go to pos
//2:	Meet with mates(-> go to middle)
//           Somebody found goal: 2(-> go to goal)
//           Nobody found goal: backto 0

public class RobotBehaviourTeam1 : RobotBehaviour
{
    public List<Tuple<RobotScript, Vector3>> tmpRobotPositions = new List<Tuple<RobotScript, Vector3>>();
    public List<RobotScript> tmpFoundRobots = new List<RobotScript>();
    public int state = 0;
    public RobotBehaviourTeam1(RobotScript robot) : base(robot) { }

    public override void DoStep()
    {
        if (state == 0)
        {
            robot.SetTargetLocation(new Vector3(UnityEngine.Random.Range(-22, 22), 0, UnityEngine.Random.Range(-22, 22)));
            state = 1;
        }
        else if (state == 1)
        {
            if (!robot.moving || robot.foundGoal) // reached end or found goal!
            {
                state = 2;
                robot.SetTargetLocation(new Vector3(0, 0, 0));
            }
        }
        else if (state == 2)
        {
            
            tmpRobotPositions.Clear();
            robot.Broadcast<string>(new Message<string>("AnybodyHere?")); // Update Robot Positions

            float range = ControllerScript.ctrlScript.communicateRange;
            Vector3 closest_robot_pos = Vector3.negativeInfinity;
            for (int i = 0; i < tmpRobotPositions.Count; i++)
            {
                float dist = Vector3.Distance(robot.rb.position, tmpRobotPositions[i].Item2);
                if (dist < range)
                {
                    range = dist;
                    closest_robot_pos = tmpRobotPositions[i].Item2;
                }
            }

            if (range < ControllerScript.ctrlScript.communicateRange - 1f && closest_robot_pos != Vector3.negativeInfinity)
            {
                Vector3 dir = robot.rb.position - closest_robot_pos;
                robot.SetTargetLocation(robot.rb.position + dir);
            }
            else if (range < ControllerScript.ctrlScript.communicateRange && closest_robot_pos != Vector3.negativeInfinity)
            {
                robot.SetTargetLocation(robot.rb.position);
            }
            else if (robot.moving == false)
            {
                robot.SetTargetLocation(new Vector3(0, 0, 0));
            }
        }


    }

    public override void RecieveMessage<T>(Message<T> m, RobotScript sender)
    {
        if(m.GetData() is string)
        {
            if("AnybodyHere?".Equals(m.GetData()))
            {
                robot.Message<Vector3>(sender, new Message<Vector3>(robot.rb.position));
            }
        }
        else if(m.GetData() is Vector3 vector)
        {
            updateKnownRobotPosition(sender, vector);
        }
    }

    private void updateKnownRobotPosition(RobotScript robot, Vector3 pos)
    {
        for(int i = 0; i < tmpRobotPositions.Count; i++)
        {
            if(robot.Equals(tmpRobotPositions[i].Item1))
            {
                tmpRobotPositions[i] = new Tuple<RobotScript, Vector3>(robot, pos);
                return;
            }
        }
        tmpRobotPositions.Add(new Tuple<RobotScript, Vector3>(robot, pos));
    }
}
