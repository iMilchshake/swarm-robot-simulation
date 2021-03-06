using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RobotScript : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    private RobotBehaviour behaviour;
    public int id;
    public bool moving;
    private GameObject goal;
    public Vector3 goalPosition;
    public bool foundGoal;
    public Vector3 targetLocation;
    public Material defaultMaterial;
    public Material foundGoalMaterial;
    public MeshRenderer mainBodyRenderer;
    public Color innerRingColor;
    public Color outerRingColor;
    public Color communicationLineColor;
    public Color movementLineColor;

    public Vector3 nextPos;
    public bool nextPosSet;

    private static List<RobotScript> robots = new List<RobotScript>();
    public static int robot_count = 0;

    public Material lineRendererMaterial;

    void Start()
    {
        //find rb component
        rb = GetComponent<Rigidbody>();

        //add behaviour to Robot
        //behaviour = new RobotBehaviourBoidsAdvanced(this);

        //add self to robot-list
        robots.Add(this);

        //set unique id
        id = robot_count++;

        //set spawn location as movement target-location
        SetTargetLocation(rb.position);

        //find goal object in level
        goal = GameObject.Find("Goal");
        goalPosition = Vector3.negativeInfinity;
        foundGoal = false;

        //initialize nextPos
        nextPos = Vector3.zero;
        nextPosSet = false;

        //set default material
        mainBodyRenderer.material = defaultMaterial;

        //disable auto physics
        Physics.autoSimulation = true;
    }

    void Update()
    {
        //run behaviour's decision making
        behaviour.DoStep();

        //Apply Movement
        Vector3 dirVec = new Vector3(targetLocation.x - rb.position.x, 0, targetLocation.z - rb.position.z);
        if (Vector3.Distance(Vector3.zero, dirVec) > speed)
        {
            dirVec = dirVec.normalized * speed;
            nextPos = transform.position + dirVec;
            nextPosSet = true;
            //rb.MovePosition(transform.position + dirVec);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dirVec), 45f);
            moving = true;
        }
        else
        {
            transform.position = new Vector3(targetLocation.x, rb.position.y, targetLocation.z);
            moving = false;
        }

        //test if goal was found
        if (!foundGoal && Vector3.Distance(goal.transform.position, transform.position) <
            ControllerScript.ctrlScript.visionRange)
        {
            GoalFound(goal.transform.position);
        }
    }

    private void LateUpdate()
    {
        // apply motion if possible
        if (nextPosSet)
        {
            transform.position = nextPos;
        }

        // check if robot is out of bounds!
        Vector3 oldpos = transform.position;
        if (Math.Abs(transform.position.x) > 24)
        {
            transform.position = new Vector3(Math.Sign(transform.position.x) * 24, oldpos.y, oldpos.z);
        }
        else if (Math.Abs(transform.position.z) > 24)
        {
            transform.position = new Vector3(oldpos.x, oldpos.y, Math.Sign(transform.position.z) * 24);
        }

        //show debug
        if (ControllerScript.ctrlScript.debug)
            ShowDebug();
    }

    public void GoalFound(Vector3 pos)
    {
        goalPosition = pos;
        foundGoal = true;
        mainBodyRenderer.material = foundGoalMaterial;
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
        foreach (RobotScript rs in FindRobots())
            Message(rs, m);
    }

    public List<RobotScript> FindRobots()
    {
        List<RobotScript> robotsInRange = new List<RobotScript>();
        foreach (RobotScript rs in robots)
        {
            if (rs != this && Vector3.Distance(this.rb.position, rs.rb.position) <
                ControllerScript.ctrlScript.communicateRange)
            {
                robotsInRange.Add(rs);
            }
        }

        return robotsInRange;
    }

    private void ShowDebug()
    {
        foreach (var rs in FindRobots())
            TobiDraw.tobiDraw.DrawLine(rb.position, rs.rb.position, communicationLineColor, 0.15f);
        
        TobiDraw.tobiDraw.DrawLine(
            rb.position,
            new Vector3(targetLocation.x, transform.position.y, targetLocation.z),
            movementLineColor,
            0.1f);

        TobiDraw.tobiDraw.DrawCircle(rb.position, ControllerScript.ctrlScript.communicateRange, 0.05f, 64, outerRingColor);
        TobiDraw.tobiDraw.DrawCircle(rb.position, ControllerScript.ctrlScript.visionRange, 0.05f, 64, innerRingColor);
    }

    public void DestroyRobot()
    {
        robots.Remove(this);
        Destroy(this.gameObject);
    }

    public void SetBehaviour(BehaviourType b)
    {
        switch (b)
        {
            case BehaviourType.Boids1:
                behaviour = new RobotBehaviourBoids(this);
                break;
            case BehaviourType.Boids2:
                behaviour = new RobotBehaviourBoidsAdvanced(this);
                break;
            case BehaviourType.Net:
                behaviour = new RobotBehaviourNetAdvanced(this);
                break;
            case BehaviourType.SoloCoop:
                behaviour = new RobotBehaviourSoloCooperation(this);
                break;
            case BehaviourType.MinCheating:
                behaviour = new RobotBehaviourSoloCheating(this);
                break;
            default:
                Debug.LogError("Invalid Behaviour name");
                break;
        }
    }
}

public enum BehaviourType
{
    Boids1 = 0,
    Boids2 = 1,
    Net = 2,
    SoloCoop = 3,
    MinCheating = 4
}