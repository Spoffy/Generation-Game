using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    public int tickRate = 1;
    private int tickCount = 0;
    
    public const int TICK_PRIORITY_LEVELS = 10;
    public List<ITickable>[] tickables = new List<ITickable>[TICK_PRIORITY_LEVELS];

    public Ticker()
    {
        for (var i = 0; i < TICK_PRIORITY_LEVELS; i++)
        {
            tickables[i] = new List<ITickable>();
        }
    }
    
    public static Ticker FindTicker()
    {
        return GameObject.FindWithTag("Ticker").GetComponent<Ticker>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tickCount += 1;
        if (tickCount >= tickRate)
        {
            tickCount = 0;
            foreach(var priorityLevel in tickables)
            {
                foreach (var tickable in priorityLevel)
                {
                    tickable.Tick();
                }
            }
        }
    }

    public void Register(ITickable tickable, int priority = -1)
    {
        priority = priority < 0 ? TICK_PRIORITY_LEVELS - 1 : priority;
        tickables[priority].Add(tickable);
    }
}

public interface ITickable
{
    void Tick();
}
