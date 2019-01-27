using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Update is called once per frame
    void Update()
    {
        var textOutput = GetComponentInChildren<Text>();
        if (textOutput)
        {
            textOutput.text = string.Format("{0:0}", resourceGeneration[ResourceType.Power]);
        }
    }

    public void Tick()
    {
        Debug.Log("Generator ticked");
        var storage = GetComponent<ResourceStorage>();
        foreach (var pair in resourceGeneration)
        {
            storage.resources[pair.Key] = pair.Value;
        }
        
        Debug.Log("Generator has " + storage.resources[ResourceType.Power]);

        var placeable = GetComponent<Placeable>();

        foreach (var connectedPlaceable in placeable.connected)
        {
            var conduit = connectedPlaceable.GetComponent<ResourceConduit>();
            if (conduit != null)
            {
                conduit.flowFrom(storage);
            }
        }
    }
}
