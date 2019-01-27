using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Colony : MonoBehaviour, ITickable
{
    public const int COLONY_TICK_PRIORITY = 9;
    public Text materialsText;
    public Text goalText;
    public Text peopleText;
    public int goalQuantity = 100;
    public ResourceDictionary resources = new ResourceDictionary();
    // Start is called before the first frame update
    void Start()
    {
        resources.populate();
        Ticker.FindTicker().Register(this, COLONY_TICK_PRIORITY);
        goalText.text = string.Format("{0} people", goalQuantity);
    }

    // Update is called once per frame
    void Update()
    {
        //materialsText.text = string.Format("{0}",resources[ResourceType.Materials]);
        peopleText.text = string.Format("{0}", resources[ResourceType.People]);
    }

    public void Tick()
    {
        var constructorObjects = GameObject.FindGameObjectsWithTag("Constructor");
        foreach (var constructorObject in constructorObjects)
        {
            if (ResourceConsumer.isFunctioning(constructorObject))
            {
                var constructorStorage = constructorObject.GetComponent<ResourceStorage>();
                resources[ResourceType.Materials] += constructorStorage.resources[ResourceType.Materials];
            }
        }

        resources[ResourceType.People] = 0;
        foreach (var houseObject in GameObject.FindGameObjectsWithTag("House"))
        {
            if (ResourceConsumer.isFunctioning(houseObject))
            {
                var houseStorage = houseObject.GetComponent<ResourceStorage>();
                resources[ResourceType.People] += houseStorage.resources[ResourceType.People];
            }
        }
    }
}