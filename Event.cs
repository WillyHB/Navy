/*
using System.Collections.Generic;
namespace Navy
{

    public class Event<T>
    {
        public Event()
        {
            
        }

        private Dictionary<Action<T>, Event> subscribedDelegates = new();
        private event Action<T> e;

        public void Invoke(T value)
        {
            e?.Invoke(value);
        }

        public void Subscribe(Action<T> value, Event unsubscribe = null)
        {
            subscribedDelegates.Add(value, unsubscribe);
            unsubscribe?.Subscribe(() => Unsubscribe(value));
            e += value;
        }

        public void Unsubscribe(Action<T> value)
        {
            if (subscribedDelegates.TryGetValue(value, out var unsubscribe))
            {
                unsubscribe.Unsubscribe(() => Unsubscribe(value));
            }

            subscribedDelegates.Remove(value);
            e -= value;
        }

        public bool IsEmpty()
        {
            return e == null;
        }
    }



    public class Event
    {
        public Event()
        {

        }

        private Dictionary<Action, Event> subscribedDelegates = new();
        private event Action e;

        public void Invoke()
        {
            e?.Invoke();
        }

        public void Subscribe(Action value, Event unsubscribe = null)
        {
            subscribedDelegates.Add(value, unsubscribe);
            unsubscribe?.Subscribe(() => Unsubscribe(value));
            e += value;
        }

        public void Unsubscribe(Action value)
        {
            if (subscribedDelegates.TryGetValue(value, out var unsubscribe))
            {
                unsubscribe.Unsubscribe(() => Unsubscribe(value));
            }

            subscribedDelegates.Remove(value);
            e -= value;
        }

        public bool IsEmpty()
        {
            return e == null;
        }
    }
}
*/