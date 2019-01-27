using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
public class ResourceEndpoint : ResourceReceiver
{
    private ResourceStorage storage;
    
    void Start()
    {
        storage = GetComponent<ResourceStorage>();
    }

    public override void flowFrom(ResourceStorage sourceStorage, int sourceFlowIteration)
    {
        foreach (var resourcePair in sourceStorage.resources)
        {
            storage.resources[resourcePair.Key] += resourcePair.Value;
        }
        
    }
}
