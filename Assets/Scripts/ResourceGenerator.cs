using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Placeable))]
[RequireComponent(typeof(ResourceStorage))]
public class ResourceGenerator : MonoBehaviour, ITickable
{
    public const int GENERATOR_TICK_PRIORITY = 1;
    public ResourceDictionary resourceGeneration = new ResourceDictionary();
    // Start is called before the first frame update
    void Start()
    {
        resourceGeneration.populate();
        Ticker.FindTicker().Register(this, GENERATOR_TICK_PRIORITY);
    }

    public void Tick()
    {
        if (ResourceConsumer.isFunctioning(gameObject))
        {
            return;
        }
        
        var storage = GetComponent<ResourceStorage>();
        var placeable = GetComponent<Placeable>();
        foreach (var pair in resourceGeneration)
        {
            storage.resources[pair.Key] = pair.Value;
            foreach (var conduit in Placeable.FindConnectedTo<ResourceReceiver>(placeable))
            {
                conduit.flowFrom(pair.Key, pair.Value, Ticker.FindTicker().NextFlow());
            }
        }
        

    }
}
