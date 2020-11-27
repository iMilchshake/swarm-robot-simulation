using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ControllerScript : MonoBehaviour
{
    public static ControllerScript ctrlScript;
    public static Random rnd;
    public int fps;
    public float communicateRange;
    public float visionRange;
    public GameObject spawnObject;
    public GameObject robotPrefab;
    
    void Start()
    {
        //make this single instance of ControllerScript accessible to everyone
        ctrlScript = this;

        //set target fps
        Application.targetFrameRate = fps;

        //set timeScale
        Time.timeScale = 1;
        
        //initialize random generator
        rnd = new Random();
        
        //Spawn Robots
        SpawnRobots(9, 3, 1.2f);
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
                    GameObject.Instantiate(robotPrefab, spawnPos, Quaternion.identity);
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
    
    
}
