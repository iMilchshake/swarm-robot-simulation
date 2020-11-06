using UnityEngine;
﻿using System.Collections;
using System.Collections.Generic;

public class RobotBehaviourTest : RobotBehaviour
{
    public RobotBehaviourTest(RobotScript parent) : base(parent) { }

    public override void DoStep()
    {
        parent.Message(parent, new Message<string>("asd"));
        parent.Message(parent, new Message<Vector2>(new Vector2(1, 2)));
    }

    public override void RecieveMessage<T>(Message<T> m)
    {
        if(m.GetData() is string)
        {
            Debug.Log("Recieved String: " + m.GetData());
        }
        else if(m.GetData() is Vector2)
        {
            Debug.Log("Recieved Vector2: " + m.GetData());
        }
    }
}
