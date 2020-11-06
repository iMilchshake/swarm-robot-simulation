using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message<T>
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
