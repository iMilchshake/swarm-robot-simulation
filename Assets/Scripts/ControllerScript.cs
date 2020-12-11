using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = System.Random;

public class ControllerScript : MonoBehaviour
{
    public static ControllerScript ctrlScript;
    public static Random rnd;
    public bool debug;
    public int fps;
    public float communicateRange;
    public float communicateRangeSquared;
    public float visionRange;
    public float visionRangeSquared;
    public GameObject spawnObject;
    public GameObject robotPrefab;
    private List<RobotScript> currentRobots = new List<RobotScript>();
    public float avgDirMult;
    public float avgPosDirMult;
    public float ownDirMult;
    public float collisionMult;
    public float randomnessMult;
    void Start()
    {
        //make this single instance of ControllerScript accessible to everyone
        ctrlScript = this;

        //set target fps
        Application.targetFrameRate = fps;
        
        //disable autoSimulation/physics
        Physics.autoSimulation = false;

        //calculate squared ranges
        communicateRangeSquared = communicateRange * communicateRange;
        visionRangeSquared = visionRange * visionRange;
        
        //initialize random generator
        rnd = new Random();
        
        // Start Coroutine for Robots Benchmarking
        StartCoroutine(BenchmarkRobot(5, 20, 1, 5, 1.2f, 3));
    }
    
    // TODO: Add variable RobotBehaviour
    private IEnumerator BenchmarkRobot(int nMin, int nMax, int step, int repeats, float gap, int rowSize)
    {
        // initialize output-string
        String output = "";
        
        // run simulation for each n in [nMin; nMax; step]
        for (int n = nMin; n <= nMax; n += step)
        {
            // repeat simulation multiple times
            for (int i = 0; i < repeats; i++)
            {
                Debug.Log(n);
                
                // Spawn n robots
                SpawnRobots(n, rowSize, gap);

                // start timer
                int t0 = Time.frameCount;

                // Wait for robots to finish
                var allDone = false;
                while (!allDone)
                {
                    allDone = true;
                    foreach (var robo in currentRobots)
                    {
                        if (!robo.foundGoal)
                        {
                            allDone = false;
                            break;
                        }
                    }
                    
                    //wait for next frame
                    yield return null; 
                }
                
                // end timer
                int t = Time.frameCount - t0;

                // Clear Scene
                while (currentRobots.Count > 0)
                {
                    RobotScript robo = currentRobots[0];
                    currentRobots.RemoveAt(0);
                    robo.DestroyRobot();
                }

                Debug.Log(n + " Robots finished in " + t + " frames");
                output += n + ";" + t + "\n";
            }
        }

        Debug.Log(output);
    }
    
    void SpawnRobots(int n, int rowSize, float gap)
    {
        var rows = Mathf.CeilToInt((float) n / (float)rowSize);
        var rowLength = (rows-1) * gap;
        var spawnedRobotCount = 0;
        var spawnPosition = spawnObject.transform.position;
        
        for (var r = 0; r < rows; r++)
        {
            for (var i = 0; i < rowSize; i++)
            {
                if(spawnedRobotCount++ < n)
                {
                    Vector3 spawnPos = new Vector3(spawnPosition.x + (r * gap) - (rowLength/2), spawnPosition.y, spawnPosition.z - (i * gap));
                    GameObject robotGameObject = GameObject.Instantiate(robotPrefab, spawnPos, Quaternion.Euler(0f, (float) ControllerScript.rnd.NextDouble() * 360f, 0f));
                    RobotScript robotScr = robotGameObject.GetComponent<RobotScript>();
                    currentRobots.Add(robotScr);
                }
            }
        }

    }

    public static bool RandomBoolean()
    {
        return rnd.Next(0, 2) == 0;
    }

    // returns a number between (min, max) but its negative for 50% of the time
    public static double RandomDoubleNeg(double min, double max)
    {
        if (RandomBoolean())
            return RandomDouble(min, max) * -1;
        return RandomDouble(min, max);
    }
    
    public static double RandomDouble(double min, double max)
    {
        return (rnd.NextDouble()*(max-min)) + min;

    }
    
    public static Vector2 RandomOuterPosition(int outer, int max)
    {
        var x1 = (float) RandomDoubleNeg(max - outer, max);
        var x2 = (float) RandomDoubleNeg(0, max);
        return RandomBoolean() ? new Vector2(x1, x2) : new Vector2(x2, x1);
    }

    public static bool InRangeSquared(Vector3 a, Vector3 b, float rangeSquared)
    {
        return (a - b).sqrMagnitude < rangeSquared;
    }

    public static bool RandomBooleanWithChange(double chance)
    {
        return rnd.NextDouble() < chance;
    }
}
