using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Unity.Profiling;
using UnityEngine;
using Debug = UnityEngine.Debug;
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
    public GameObject goal;
    
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
        
        //define behaviours
        BehaviourType[] behaviours = {BehaviourType.SoloCoop};
        
        // Start Coroutine for Robots Benchmarking
        StartCoroutine(BenchmarkRobot(5, 40, 2, 4, 1.2f, 3, 15, behaviours));
    }
    
    private IEnumerator BenchmarkRobot(int nMin, int nMax, int step, int repeats, float gap, int rowSize, int pCount, BehaviourType[] behaviours)
    {
        // Debug
        Debug.Log("Starting simulation..");
        Debug.Log("" + (((nMax - nMin)/step)*repeats*pCount*behaviours.Length) + " expected repeats" );
        
        // initialize array with output-strings
        string[] output = new string[behaviours.Length];
        for(int i = 0; i < behaviours.Length; i++)
        {
            output[i] = "n;frames\n";
        }
        
        // run simulation for different goal-positions
        for (int p = 0; p < pCount; p++)
        {
            // set random goal position
            float new_x = (float) RandomDouble(-21, 21);
            float new_z = (float) RandomDouble(-21, 10);
            goal.transform.position = new Vector3(new_x, goal.transform.position.y, new_z);

            // run simulation for each behaviour
            for (var b = 0; b < behaviours.Length; b++)
            {
                Debug.Log("Goal Position: " + (p + 1) + "/" + pCount + " | " + "Behaviour: " + behaviours[b] + " ("+ (b + 1) + "/" + behaviours.Length + ")");
                // run simulation for each n in [nMin; nMax; step]
                for (int n = nMin; n <= nMax; n += step)
                {
                    // only spawn one
                    if (behaviours[b].Equals("min_cheating"))
                        n = 1;
                    
                    // repeat simulation multiple times
                    for (int i = 0; i < repeats; i++)
                    {
                        // Spawn n robots
                        SpawnRobots(n, rowSize, gap, behaviours[b]);

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
                        
                        output[b] += n + ";" + t + "\n";
                    }
                
                    // skip other n sizes
                    if (behaviours[b].Equals("min_cheating"))
                        n = nMax;
                }
            }
        }

        var settings = communicateRange + ";" + visionRange + ";" + avgDirMult + ";" + avgPosDirMult + ";" +
                       ownDirMult + ";" + collisionMult + ";" + randomnessMult + "\n";
        

        Array.ForEach(output, Debug.Log);
        
        var id = Guid.NewGuid().ToString("N").Substring(0, 5);
        var configPath = Application.dataPath + "/csv/_configs.txt";
        
        // save data for each behaviour
        for (var b = 0; b < behaviours.Length; b++)
        {
            var dataPath = Application.dataPath + "/csv/" + behaviours[b] + "_" + id + ".csv";
            var writer = new StreamWriter(dataPath);
            writer.WriteLine(output[b]);
            writer.Flush();
            writer.Close();
        }
        
        // save config
        var writer2 = File.AppendText(configPath);//new StreamWriter(configPath);
        writer2.WriteLine(id + " -> " + settings);
        writer2.Flush();
        writer2.Close();
    }
    
    void SpawnRobots(int n, int rowSize, float gap, BehaviourType behaviour)
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
                    robotScr.SetBehaviour(behaviour);
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
    
    // Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
    public static string ColorToHex(Color32 color)
    {
        var hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }
 
    public static Color HexToColor(string hex)
    {
        var r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
        var g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
        var b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r,g,b, 255);
    }

}
