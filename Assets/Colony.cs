using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Colony : MonoBehaviour, ITickable
{
    public const int COLONY_TICK_PRIORITY = 9;
    public Text materialsText;
    public ResourceDictionary resources = new ResourceDictionary();
    // Start is called before the first frame update
    void Start()
    {
        resources.populate();
        Ticker.FindTicker().Register(this, COLONY_TICK_PRIORITY);
    }

    // Update is called once per frame
    void Update()
    {
        materialsText.text = string.Format("{0}",resources[ResourceType.Materials]);
    }

    public void Tick()
    {
        Debug.Log("Colony ticked");
        var constructorObjects = GameObject.FindGameObjectsWithTag("Constructor");
        foreach (var constructorObject in constructorObjects)
        {
            Debug.Log("Constructor found");
            if (ResourceConsumer.isFunctioning(constructorObject))
            {
                var constructorStorage = constructorObject.GetComponent<ResourceStorage>();
                resources[ResourceType.Materials] += constructorStorage.resources[ResourceType.Materials];
            }
        }
    }
}