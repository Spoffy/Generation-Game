using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Experimental.UIElements;
using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
public class ResourceEndpoint : ResourceReceiver
{
    public List<ResourceType> filterList = new List<ResourceType>();
    private ResourceStorage storage;
    
    void Start()
    {
        storage = GetComponent<ResourceStorage>();
    }

    public override void flowFrom(ResourceType resourceType, float quantity, int sourceFlowIteration)
    {
        if (filterList.Contains(resourceType)) { return; }
        storage.resources[resourceType] += quantity;
        
        if(!ResourceConsumer.isFunctioning(gameObject))
        {
            return;
        }
    }
}
