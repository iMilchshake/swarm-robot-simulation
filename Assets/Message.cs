<<<<<<< HEAD
﻿public class Message<T>
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message<T>
>>>>>>> 0ca9b4d7e5fa2e20f2289900a7ffaa5470ceff30
{
    private T data;
    public Message(T data)
    {
        this.data = data;
    }

    public T GetData()
    {
        return data;
    }
}
