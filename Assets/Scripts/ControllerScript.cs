using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    public static ControllerScript ctrlScript;
    public int fps;
    public float communicateRange;
    public float visionRange;

    void Start()
    {
        //make this single instance of ControllerScript accessible to everyone
        ctrlScript = this;

        //set target fps
        Application.targetFrameRate = fps;

        Time.timeScale = 1;
    }
}
