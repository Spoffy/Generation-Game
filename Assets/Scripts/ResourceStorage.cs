using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Placeable))]
public class ResourceStorage : MonoBehaviour, ITickable
{
    public const int STORAGE_TICK_PRIORITY = 0;
    public ResourceDictionary resources = new ResourceDictionary();
    // Start is called before the first frame update
    void Start()
    {
        resources.populate();
        Ticker.FindTicker().Register(this, STORAGE_TICK_PRIORITY);
    }

    // Wipe the storage every tick
    public void Tick()
    {
        Debug.Log("Storage Ticked");
        var keys = new List<ResourceType>();
        keys.AddRange(resources.Keys);
        foreach(var resource in keys)
        {
            resources[resource] = 0;
        }
    }
}
