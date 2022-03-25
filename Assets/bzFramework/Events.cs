using System;
using System.Collections.Generic;

namespace BZFramework
{
    public class BZNotification
    {

    }

    public class BZEvent<T> : BZNotification
    {
        public T EventData { get; protected set; }

        public BZEvent(T data)
        {
            EventData = data;
        }

    }

    public sealed class BZEventDispatcher
    {
        public delegate void BZEventHandler<T>(T anEvent) where T : BZNotification;

        public static BZEventDispatcher Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Post<T>(T anItem) where T : BZNotification
        {
            List<Delegate> calls = new List<Delegate>();
            lock (_lockObj)
            {
                foreach (Type keyType in _handlers.Keys)
                {
                    if (keyType.IsAssignableFrom(anItem.GetType()))
                    {
                        foreach ((Delegate d, bool derived) in _handlers[keyType])
                        {
                            if ((derived) || (keyType == anItem.GetType()))
                            {
                                calls.Add(d);
                            }
                        }
                    }
                }
            }
            if (calls.Count > 0)
            {
                foreach (Delegate ad in calls)
                {
                    ad?.DynamicInvoke(anItem);
                }
            }
        }

        public void Subscribe<T>(BZEventHandler<T> aHandler, bool IncludeDerived = false) where T : BZNotification
        {
            lock (_lockObj)
            {
                if (!_handlers.ContainsKey(typeof(T)))
                {
                    _handlers[typeof(T)] = new List<(Delegate, bool)>();
                }
                if (!_handlers[typeof(T)].Contains((aHandler, IncludeDerived)))
                {
                    _handlers[typeof(T)].Add((aHandler, IncludeDerived));
                }
            }
        }

        public void Unsubscribe<T>(BZEventHandler<T> aHandler) where T : BZNotification
        {
            lock (_lockObj)
            {
                if (_handlers.ContainsKey(typeof(T)))
                {
                    for (int idx = _handlers[typeof(T)].Count - 1; idx >= 0; idx--)
                    {
                        if (_handlers[typeof(T)][idx].Item1 == (Delegate)aHandler)
                        {
                            _handlers[typeof(T)].RemoveAt(idx);
                        }
                    }
                }
            }
        }

        private static readonly BZEventDispatcher _instance = new BZEventDispatcher();
        private Dictionary<Type, List<(Delegate, bool)>> _handlers = new Dictionary<Type, List<(Delegate, bool)>>();
        private static readonly object _lockObj = new object();

    }
}