using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Collections
{

    public class BaseConnection<T> : IConnection<T>
    {
        public float cost = 1;
        protected T fromNode;
        protected T toNode;

        public BaseConnection(T from, T to)
        {
            this.fromNode = from;
            this.toNode = to;
        }

        public virtual float GetCost()
        {
            return cost;
        }

        public T GetFromNode()
        {
            return fromNode;
        }

        public T GetToNode()
        {
            return toNode;
        }

        public bool HasNode(T t)
        {
            return t != null && (t.Equals(fromNode) || t.Equals(toNode));
        }
    }
}