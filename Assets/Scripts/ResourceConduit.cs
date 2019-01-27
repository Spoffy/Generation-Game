using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Placeable))]
[RequireComponent(typeof(ResourceStorage))]
public class ResourceConduit : MonoBehaviour, ITickable
{
    public const int CONDUIT_TICK_PRIORITY = 5;
    public ResourceDictionary resourceFalloff = new ResourceDictionary();

    private ResourceStorage storage;

    private int flowIteration = 0;
    // Start is called before the first frame update
    void Start()
    {
        resourceFalloff.populate();
        storage = GetComponent<ResourceStorage>();
        Ticker.FindTicker().Register(this, CONDUIT_TICK_PRIORITY);
    }

    public void Tick()
    {
        flowIteration = 0;
    }

    public void flowFrom(ResourceStorage sourceStorage, int sourceFlowIteration)
    {
        if (sourceFlowIteration <= flowIteration)
        {
            return;
        }
        flowIteration = sourceFlowIteration;
        
        foreach(var resourcePair in sourceStorage.resources)
        {
            var falloff = resourceFalloff[resourcePair.Key];
            storage.resources[resourcePair.Key] += resourcePair.Value * falloff;
        }

        foreach (var conduit in ResourceConduit.FindConnectedTo(GetComponent<Placeable>()))
        {
            conduit.flowFrom(storage, flowIteration);
        }
        
    }

    public static List<ResourceConduit> FindConnectedTo(Placeable source)
    {
        var results = new List<ResourceConduit>();
        foreach (var connectedPlaceable in source.connected)
        {
            var conduit = connectedPlaceable.GetComponent<ResourceConduit>();
            if (conduit != null)
            {
                results.Add(conduit);
            }
        }

        return results;
    }
}
