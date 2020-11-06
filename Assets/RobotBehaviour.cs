<<<<<<< HEAD
﻿public abstract class RobotBehaviour
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RobotBehaviour
>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30
{
    public RobotScript parent;

    protected RobotBehaviour(RobotScript parent)
    {
        this.parent = parent;
    }


    public abstract void RecieveMessage<T>(Message<T> m);
    public abstract void DoStep();
}
