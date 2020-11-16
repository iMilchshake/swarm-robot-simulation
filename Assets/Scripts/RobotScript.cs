using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RobotScript : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public RobotBehaviour behaviour;
    public MonoScript test;
    public int id;
    public bool moving;
    private GameObject goal;
    public Vector3 goalPosition;
    public bool foundGoal;
    public Vector3 targetLocation;

    private static List<RobotScript> robots = new List<RobotScript>();
    public static int robot_count = 0;

    void Start()
    {
        //find rb component
        rb = GetComponent<Rigidbody>();

        //add behaviour to Robot
        behaviour = new RobotBehaviourTeam1(this);

        //add self to robot-list
        robots.Add(this);

        //set unique id
        id = robot_count++;

        //set spawn location as movement target-location
        SetTargetLocation(rb.position);

        //find goal object in lvl
        goal = GameObject.Find("Goal");
        goalPosition = Vector3.negativeInfinity;
        foundGoal = false;
    }

    void FixedUpdate()
    {
        //run behaviour's decision making
        behaviour.DoStep();

        //Apply Movement
        Vector3 dirVec = new Vector3(targetLocation.x - rb.position.x, 0, targetLocation.z - rb.position.z);
        if (Vector3.Distance(Vector3.zero, dirVec) > speed)
        {
            dirVec = dirVec.normalized * speed;
            rb.MovePosition(rb.position + dirVec);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dirVec), 15f);
            moving = true;
        } else
        {
            rb.MovePosition(new Vector3(targetLocation.x, rb.position.y, targetLocation.z));
            moving = false;
        }

        //show debug
        ShowDebug();

        //test if goal was found
        if (!foundGoal && Vector3.Distance(goal.transform.position, transform.position) < ControllerScript.ctrlScript.visionRange)
        {
            goalPosition = goal.transform.position;
            foundGoal = true;
        } 
    }

    public void SetTargetLocation(Vector3 pos)
    {
        targetLocation = pos;
    }

    public void Message<T>(RobotScript recv, Message<T> m)
    {
        recv.behaviour.RecieveMessage(m, this);
    }

    public void Broadcast<T>(Message<T> m)
    {
        foreach (RobotScript rs in findRobots())
            Message(rs, m);
    }

    public List<RobotScript> findRobots()
    {
        List<RobotScript> robotsInRange = new List<RobotScript>();
        foreach (RobotScript rs in robots)
        {
            if (rs != this && Vector3.Distance(this.rb.position, rs.rb.position) < ControllerScript.ctrlScript.communicateRange)
            {
                robotsInRange.Add(rs);
            }
        }
        return robotsInRange;
    }

    private void ShowDebug()
    {
        foreach (var rs in findRobots())
            Debug.DrawLine(rb.position, rs.rb.position, new Color(0.5f, 0.2f, 0.2f, 0.5f));

        tDebug.DrawEllipse(rb.position, Vector3.up, Vector3.forward, ControllerScript.ctrlScript.communicateRange, ControllerScript.ctrlScript.communicateRange, 32, new Color(0.5f, 0.2f, 0.2f, 0.5f));
        tDebug.DrawEllipse(rb.position, Vector3.up, Vector3.forward, ControllerScript.ctrlScript.visionRange, ControllerScript.ctrlScript.visionRange, 32, new Color(0.2f, 0.6f, 0.3f, 0.5f));

        Debug.DrawLine(rb.position, new Vector3(targetLocation.x, rb.position.y, targetLocation.z), new Color(0.2f, 0.6f, 0.3f, 0.5f));
    }


}