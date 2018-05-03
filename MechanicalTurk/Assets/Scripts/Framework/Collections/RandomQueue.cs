// Shane McDermott 2018

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Collections
{
    /** 
     * Generic queue implementation that includes random element removal
     */
    public class RandomQueue<T>
    {
        protected List<T> contents = new List<T>();

        public void Push(T item)
        {
            contents.Add(item);
        }

        public void RemoveAt(int index)
        {
            contents.RemoveAt(index);
        }

        public T PopRandom()
        {
            int index = Random.Range(0, contents.Count);
            T t = contents[index];
            RemoveAt(index);
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
}