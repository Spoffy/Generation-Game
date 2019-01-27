using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : MonoBehaviour, ITickable
{
    public ResourceDictionary resources = new ResourceDictionary();
    // Start is called before the first frame update
    void Start()
    {
        resources.populate();
        Ticker.FindTicker().Register(this);
    }

    public void Tick()
    {
        Debug.Log("Storage Ticked");
    }
}
