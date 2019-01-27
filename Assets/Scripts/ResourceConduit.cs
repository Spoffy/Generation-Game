using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Placeable))]
[RequireComponent(typeof(ResourceStorage))]
public class ResourceConduit : ResourceReceiver, ITickable
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

    public override void flowFrom(ResourceType resourceType, float quantity, int sourceFlowIteration)
    {
        if (sourceFlowIteration <= flowIteration)
        {
            return;
        }
        flowIteration = sourceFlowIteration;
        
        var falloff = resourceFalloff[resourceType];
        var newQuantity = quantity * falloff;
        storage.resources[resourceType] += newQuantity;
        
        if (ResourceConsumer.isFunctioning(gameObject))
        {
            return;
        }

        foreach (var conduit in Placeable.FindConnectedTo<ResourceReceiver>(GetComponent<Placeable>()))
        {
            conduit.flowFrom(resourceType, newQuantity, flowIteration);
        }
        
    }
}
