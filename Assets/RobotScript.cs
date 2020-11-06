using System.Collections.Generic;
using UnityEngine;

public class RobotScript : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public RobotBehaviour behaviour;
    public int id;

    public Vector3 targetLocation;

    private static List<RobotScript> robots = new List<RobotScript>();
    public static int robot_count = 0;

    void Start()
    {
        //find rb component
        rb = GetComponent<Rigidbody>();

        //add behaviour to Robot
        behaviour = new RobotBehaviourTest(this);

        //add self to robot-list
        robots.Add(this);

        //set unique id
        id = robot_count++;

        //set spawn location as movement target-location
        SetTargetLocation(rb.position);
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
        } else
        {
            rb.MovePosition(new Vector3(targetLocation.x, rb.position.y, targetLocation.z));
        }

        //show debug
        ShowDebug();
    }

    public void SetTargetLocation(Vector3 pos)
    {
        targetLocation = pos;
    }

    public void Message<T>(RobotScript recv, Message<T> m)
    {
        recv.behaviour.RecieveMessage(m);
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
        foreach (RobotScript rs in findRobots())
            Debug.DrawLine(rb.position, rs.rb.position);

        tDebug.DrawEllipse(rb.position, Vector3.up, Vector3.forward, ControllerScript.ctrlScript.communicateRange, ControllerScript.ctrlScript.communicateRange, 32, Color.white);
    }


}
