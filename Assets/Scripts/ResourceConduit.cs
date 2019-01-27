﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ResourceStorage))]
public class ResourceConduit : MonoBehaviour, ITickable
{
    public const int CONDUIT_TICK_PRIORITY = 5;

    private ResourceStorage storage;
    // Start is called before the first frame update
    void Start()
    {
        storage = GetComponent<ResourceStorage>();
        Ticker.FindTicker().Register(this, CONDUIT_TICK_PRIORITY);
    }
    
    void Update()
    {
        var textOutput = GetComponentInChildren<Text>();
        if (textOutput)
        {
            var power = storage.resources[ResourceType.Power];
            textOutput.text = string.Format("{0:0}", power);
        }
    }

    public void Tick()
    {
    }

    public void flowFrom(ResourceStorage sourceStorage)
    {
        foreach(var resourcePair in sourceStorage.resources)
        {
            Debug.Log("Flowing " + resourcePair.Key + " " + resourcePair.Value);
            storage.resources[resourcePair.Key] = resourcePair.Value;
        }
    }
}
