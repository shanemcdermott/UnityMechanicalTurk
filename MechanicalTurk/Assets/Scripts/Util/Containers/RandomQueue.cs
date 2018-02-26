using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomQueue<T>
{
    protected List<T> contents = new List<T>();

    public void Push(T item)
    {
        contents.Add(item);
    }

    public T Pop()
    {
        int index = Random.Range(0, contents.Count);
        T t= contents[index];
        contents.RemoveAt(index);
        return t;
    }

    public int Count()
    {
        return contents.Count;
    }

    public bool Empty()
    {
        return contents.Count == 0;
    }

}
