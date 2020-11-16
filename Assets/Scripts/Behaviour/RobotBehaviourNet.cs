using UnityEngine;
﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.PlayerLoop;

public class RobotBehaviourNet : RobotBehaviour
{
    public List<Tuple<RobotScript, Vector3>> knownRobotPositions = new List<Tuple<RobotScript, Vector3>>();

    public RobotBehaviourNet(RobotScript robot) : base(robot) { }

    public override void DoStep()
    {
        knownRobotPositions.Clear();
        robot.Broadcast<string>(new Message<string>("AnybodyHere?", 0)); // Update Robot Positions

        float range = ControllerScript.ctrlScript.communicateRange;
        Vector3 closest_robot_pos = Vector3.negativeInfinity;
        for (int i = 0; i < knownRobotPositions.Count; i++)
        {
            float dist = Vector3.Distance(robot.rb.position, knownRobotPositions[i].Item2);
            if(dist < range)
            {
                range = dist;
                closest_robot_pos = knownRobotPositions[i].Item2;
            }
        }

        if(range < ControllerScript.ctrlScript.communicateRange - 1f && closest_robot_pos != Vector3.negativeInfinity)
        {
            Vector3 dir = robot.rb.position - closest_robot_pos;
            robot.SetTargetLocation(robot.rb.position + dir);
        }
        else if(range < ControllerScript.ctrlScript.communicateRange && closest_robot_pos != Vector3.negativeInfinity)
        {
            robot.SetTargetLocation(robot.rb.position);
        }
        else if(robot.moving == false)
        {
            robot.SetTargetLocation(new Vector3(UnityEngine.Random.Range(-22, 22), 0, UnityEngine.Random.Range(-22, 22)));
        }
    }

    public override void RecieveMessage<T>(Message<T> m, RobotScript sender)
    {
        if(m.GetData() is string)
        {
            if("AnybodyHere?".Equals(m.GetData()))
            {
                robot.Message<Vector3>(sender, new Message<Vector3>(robot.rb.position, 0));
            }
        }
        else if(m.GetData() is Vector3 vector)
        {
            updateKnownRobotPosition(sender, vector);
        }
    }

    private void updateKnownRobotPosition(RobotScript robot, Vector3 pos)
    {
        for(int i = 0; i < knownRobotPositions.Count; i++)
        {
            if(robot.Equals(knownRobotPositions[i].Item1))
            {
                knownRobotPositions[i] = new Tuple<RobotScript, Vector3>(robot, pos);
                return;
            }
        }
        knownRobotPositions.Add(new Tuple<RobotScript, Vector3>(robot, pos));
    }
}
