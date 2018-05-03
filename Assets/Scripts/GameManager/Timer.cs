using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    
    private class TimedEvent
    {
        public float InitialTimeToExecute;
        public Callback Method;
        public int Iterations;
        internal float TimeToAddPerIteration;
    }

    private List<TimedEvent> events;

    public delegate void Callback();

    private void Awake()
    {
        events = new List<TimedEvent>();
    }

    public void Add(Callback method, float inSeconds, int iterations = 1)
    {
        events.Add(new TimedEvent
        {
            Method = method,
            InitialTimeToExecute = Time.time + inSeconds,
            Iterations = iterations,
            TimeToAddPerIteration = inSeconds
        });
    }

    private void Update()
    { 
        if (events.Count == 0)
        {
            return;
        }

        for (int i = 0; i < events.Count; i++)
        {
            TimedEvent timedEvent = events[i];
            if (Time.time >= timedEvent.InitialTimeToExecute)
            {
                timedEvent.Method();
                timedEvent.Iterations--;
                if (timedEvent.Iterations <= 0)
                {
                    events.Remove(timedEvent);
                }
                else
                {
                    timedEvent.InitialTimeToExecute = Time.time + timedEvent.TimeToAddPerIteration;
                }
            }
        }
    }
}
