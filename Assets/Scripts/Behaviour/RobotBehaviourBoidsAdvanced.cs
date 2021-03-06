﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = System.Random;
using Vector3 = UnityEngine.Vector3;

public class RobotBehaviourBoidsAdvanced : RobotBehaviour
{
    public RobotBehaviourBoidsAdvanced(RobotScript robot) : base(robot) { }

    public override void DoStep()
    {
        if (robot.foundGoal)
        {
            robot.SetTargetLocation(robot.transform.position);
            robot.Broadcast<Vector3>(new Message<Vector3>(robot.goalPosition, 0));
        }
        else
        {
            List<RobotScript> nearRobots = robot.FindRobots();
            var robTransform = robot.transform;
            var target = robTransform.position;
            
            if (nearRobots.Count > 0)
            {
                // get average dir
                Vector3 avgDir = Vector3.zero;
                for (int i = 0; i < nearRobots.Count; i++)
                {
                    avgDir += nearRobots[i].transform.forward;
                }

                avgDir /= nearRobots.Count;

                // get average pos
                Vector3 avgPos = Vector3.zero;
                for (int i = 0; i < nearRobots.Count; i++)
                {
                    avgPos = nearRobots[i].transform.position;
                }

                avgPos /= nearRobots.Count;
                var avgPosDir = avgPos - robot.transform.position;
                
                // find nearest robot
                var range = ControllerScript.ctrlScript.communicateRange;
                var closestRobotPos = Vector3.negativeInfinity;
                var closestRobotFound = false;
                for (var i = 0; i < nearRobots.Count; i++)
                {
                    var dist = Vector3.Distance(robot.transform.position, nearRobots[i].transform.position);
                    if (dist < range)
                    {
                        range = dist;
                        closestRobotPos = nearRobots[i].transform.position;
                        closestRobotFound = true;
                    }
                }

                Vector3 avoidanceOrSeekingDir = Vector3.zero;
                // collision avoidance
                if (range < ControllerScript.ctrlScript.communicateRange - 2f)
                {
                    avoidanceOrSeekingDir = (robot.rb.position - closestRobotPos).normalized;
                }
                // group seeking
                else if (ControllerScript.ctrlScript.communicateRange - 1f < range && range < ControllerScript.ctrlScript.communicateRange)
                {
                    avoidanceOrSeekingDir = (closestRobotPos - robot.rb.position).normalized;
                }

                target += avgPosDir * ControllerScript.ctrlScript.avgPosDirMult;
                target += avoidanceOrSeekingDir * ControllerScript.ctrlScript.collisionMult;
                target += avgDir * ControllerScript.ctrlScript.avgDirMult;
                target += new Vector3((float) ControllerScript.rnd.NextDouble(), 0, (float) ControllerScript.rnd.NextDouble())*ControllerScript.ctrlScript.randomnessMult;
            }

            // Wall Collision
            const int layerMask = 1 << 8;
            if (Physics.Raycast(robot.transform.position, robot.transform.forward, 1f, layerMask))
            {
                // override target
                target = Vector3.zero;
            }
            else
            {
                target += robTransform.forward * ControllerScript.ctrlScript.ownDirMult;
            }

            robot.SetTargetLocation(target);
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
