
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message<T>
{
    private T data;
    private int _tag;
    
    public Message(T data, int tag)
    {
        this.data = data;
        this._tag = tag;
    }

    public T GetData()
    {
        return data;
    }

    public int GetTag()
    {
        return _tag;
    }
}
