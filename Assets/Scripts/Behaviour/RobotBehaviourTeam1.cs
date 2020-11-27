using UnityEngine;
﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.PlayerLoop;
using Random = System.Random;


//0:    find goal or robot with goal
//1:    build net until everyone is catched
//2:    everyone go to the goal

public class RobotBehaviourTeam1 : RobotBehaviour
{
    public List<Tuple<RobotScript, Vector3>> tmpRobotPositions = new List<Tuple<RobotScript, Vector3>>();
    public List<RobotScript> tmpFoundRobots = new List<RobotScript>();
    public int state = 0;
    public RobotBehaviourTeam1(RobotScript robot) : base(robot) { }

    public override void DoStep()
    {
        
        switch(state) 
        {
            // Search Goal!
            case 0:
                if (robot.foundGoal) // found goal
                {
                    // change state
                    state = 1;
                    robot.SetTargetLocation(robot.rb.position);
                }
                else if (!robot.moving) // reached targetLocation
                {
                    // move to new location
                    robot.SetTargetLocation(new Vector3(UnityEngine.Random.Range(-22, 22), 0, UnityEngine.Random.Range(-22, 22)));
                }
                break;

            // Found Goal!
            case 1:
                robot.Broadcast<Vector3>(new Message<Vector3>(robot.goalPosition, 0)); // Share Goal Position
                BuildNet();
                break;
        }
    }

    public override void RecieveMessage<T>(Message<T> m, RobotScript sender)
    {
        if(m.GetData() is string)
        {
            if("AnybodyHere?".Equals(m.GetData()))
            {
                robot.Message<Vector3>(sender, new Message<Vector3>(robot.rb.position, 1));
            }
        }
        else if(m.GetData() is Vector3 vector)
        {
            if (m.GetTag() == 0 && !robot.foundGoal) // Goal Position
            {
                robot.GoalFound(vector);
                state = 1;
                robot.SetTargetLocation(robot.rb.position);
            }
            else if (m.GetTag() == 1) // Player Position
            {
                updateKnownRobotPosition(sender, vector);
            }
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

    private void BuildNet()
    {
        tmpRobotPositions.Clear();
        robot.Broadcast<string>(new Message<string>("AnybodyHere?", 0)); // Update Robot Positions

        float range = ControllerScript.ctrlScript.communicateRange;
        Vector3 closest_robot_pos = Vector3.negativeInfinity;
        for (int i = 0; i < tmpRobotPositions.Count; i++)
        {
            float dist = Vector3.Distance(robot.rb.position, tmpRobotPositions[i].Item2);
            if(dist < range)
            {
                range = dist;
                closest_robot_pos = tmpRobotPositions[i].Item2;
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
    }
}
