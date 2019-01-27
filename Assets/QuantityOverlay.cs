using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ResourceStorage))]
public class QuantityOverlay : MonoBehaviour
{
    public Text powerDisplay;
    public Text materialDisplay;
    public Text foodDisplay;
    public Text peopleDisplay;
    private ResourceDictionary resources;
    // Start is called before the first frame update
    void Start()
    {
        var generator = GetComponent<ResourceGenerator>();
        var storage = GetComponent<ResourceStorage>();
        if (generator)
        {
            resources = generator.resourceGeneration;
        }
        else
        {
            resources = storage.resources;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Keep it upright!
        var canvas = GetComponentInChildren<Canvas>();
        canvas.gameObject.transform.rotation = Quaternion.identity;

        if (powerDisplay)
        {
            powerDisplay.text = string.Format("{0:#0}", resources[ResourceType.Power]);
        }
        
        if (materialDisplay)
        {
            materialDisplay.text = string.Format("{0:#0}", resources[ResourceType.Materials]);
        }
        
        if (foodDisplay)
        {
            foodDisplay.text = string.Format("{0:#0}", resources[ResourceType.Food]);
        }
        
        if (peopleDisplay)
        {
            peopleDisplay.text = string.Format("{0:#0}", resources[ResourceType.People]);
        }
    }
}
