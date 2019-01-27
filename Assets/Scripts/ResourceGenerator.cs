using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }

    public void Tick()
    {
        Debug.Log("Generator ticked");
    }
}
