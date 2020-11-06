<<<<<<< HEAD
﻿using System.Collections.Generic;
=======
﻿using System.Collections;
using System.Collections.Generic;
>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30
using UnityEngine;

public class RobotScript : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public RobotBehaviour behaviour;
    public int id;

<<<<<<< HEAD
    public Vector3 targetLocation;

=======
>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30
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
<<<<<<< HEAD

        //set spawn location as movement target-location
        SetTargetLocation(rb.position);
=======
>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30
    }

    void FixedUpdate()
    {
<<<<<<< HEAD
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

=======
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 tempVect = new Vector3(h, 0, v);
        tempVect = tempVect.normalized * speed;
        rb.MovePosition(transform.position + tempVect);

        //run behaviour's decision making
        behaviour.DoStep();

>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30
        //show debug
        ShowDebug();
    }

<<<<<<< HEAD
    public void SetTargetLocation(Vector3 pos)
    {
        targetLocation = pos;
    }

=======
>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30
    public void Message<T>(RobotScript recv, Message<T> m)
    {
        recv.behaviour.RecieveMessage(m);
    }

<<<<<<< HEAD
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
=======
    public List<RobotScript> findRobots()
    {
        List<RobotScript> robotsInRange = new List<RobotScript>();
        foreach(RobotScript rs in robots) {
            if(rs != this && Vector3.Distance(this.rb.position, rs.rb.position) < ControllerScript.ctrlScript.communicateRange)
>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30
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
<<<<<<< HEAD

        tDebug.DrawEllipse(rb.position, Vector3.up, Vector3.forward, ControllerScript.ctrlScript.communicateRange, ControllerScript.ctrlScript.communicateRange, 32, Color.white);
=======
>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30
    }


}
