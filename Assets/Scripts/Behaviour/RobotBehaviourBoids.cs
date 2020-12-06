using System.Collections;
using System.Collections.Generic;
using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Vector3 = UnityEngine.Vector3;

public class RobotBehaviourBoids : RobotBehaviour
{
    public RobotBehaviourBoids(RobotScript robot) : base(robot) { }

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

                // collision avoidance
                var range = ControllerScript.ctrlScript.communicateRange - 1f;
                var closestRobotPos = Vector3.negativeInfinity;
                var collisionAvoidanceDir = Vector3.zero;
                for (var i = 0; i < nearRobots.Count; i++)
                {
                    var dist = Vector3.Distance(robot.transform.position, nearRobots[i].transform.position);
                    if (dist < range)
                    {
                        range = dist;
                        closestRobotPos = nearRobots[i].transform.position;
                    }
                }

                if (range < ControllerScript.ctrlScript.communicateRange - 1f)
                {
                    collisionAvoidanceDir = (robot.rb.position - closestRobotPos).normalized;
                }
                
                target += avgPosDir * ControllerScript.ctrlScript.avgPosDirMult;
                target += collisionAvoidanceDir * ControllerScript.ctrlScript.collisionMult;
                target += avgDir * ControllerScript.ctrlScript.avgDirMult;
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
