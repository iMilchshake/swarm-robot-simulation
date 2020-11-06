using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotScript : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public RobotBehaviour behaviour;
    public int id;

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
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 tempVect = new Vector3(h, 0, v);
        tempVect = tempVect.normalized * speed;
        rb.MovePosition(transform.position + tempVect);

        //run behaviour's decision making
        behaviour.DoStep();

        //show debug
        ShowDebug();
    }

    public void Message<T>(RobotScript recv, Message<T> m)
    {
        recv.behaviour.RecieveMessage(m);
    }

    public List<RobotScript> findRobots()
    {
        List<RobotScript> robotsInRange = new List<RobotScript>();
        foreach(RobotScript rs in robots) {
            if(rs != this && Vector3.Distance(this.rb.position, rs.rb.position) < ControllerScript.ctrlScript.communicateRange)
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
    }


}
