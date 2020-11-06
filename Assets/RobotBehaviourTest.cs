<<<<<<< HEAD
﻿using UnityEngine;
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30

public class RobotBehaviourTest : RobotBehaviour
{
    public RobotBehaviourTest(RobotScript parent) : base(parent) { }

    public override void DoStep()
    {
        parent.Message(parent, new Message<string>("asd"));
<<<<<<< HEAD
        parent.Message(parent, new Message<Vector2>(new Vector2(1, 2)));
=======
        parent.Message(parent, new Message<Vector2>(new Vector2(1,2)));
>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30
    }

    public override void RecieveMessage<T>(Message<T> m)
    {
<<<<<<< HEAD
        if (m.GetData() is string)
        {
            Debug.Log("Recieved String: " + m.GetData());
        }
        else if (m.GetData() is Vector2)
=======
        if(m.GetData() is string)
        {
            Debug.Log("Recieved String: " + m.GetData());
        }
        else if(m.GetData() is Vector2)
>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30
        {
            Debug.Log("Recieved Vector2: " + m.GetData());
        }
    }
}
