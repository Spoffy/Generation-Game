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

    public override void flowFrom(ResourceType resourceType, float quantity, int sourceFlowIteration)
    {
        storage.resources[resourceType] += quantity;
    }
}
