<<<<<<< HEAD
﻿using UnityEngine;
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30

public class ControllerScript : MonoBehaviour
{
    public static ControllerScript ctrlScript;
    public int fps;
    public float communicateRange;

    void Start()
    {
        //make this single instance of ControllerScript accessible to everyone
        ctrlScript = this;

        //set target fps
        Application.targetFrameRate = fps;
    }
}
