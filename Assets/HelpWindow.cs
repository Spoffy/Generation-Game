using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class HelpWindow : MonoBehaviour
{
    public Text BuildingName;

    public Text ProducePower;
    public Text ProduceMaterial;
    public Text ProduceFood;
    public Text ProducePeople;

    public Text ConsumePower;
    public Text ConsumeMaterial;
    public Text ConsumeFood;
    public Text ConsumePeople;
    
    private Dictionary<ResourceType, Text> produceDictionary = new Dictionary<ResourceType, Text>();
    private Dictionary<ResourceType, Text> consumeDictionary = new Dictionary<ResourceType, Text>();
    // Start is called before the first frame update
    void Start()
    {
        produceDictionary[ResourceType.Power] = ProducePower;
        produceDictionary[ResourceType.Materials] = ProduceMaterial;
        produceDictionary[ResourceType.Food] = ProduceFood;
        produceDictionary[ResourceType.People] = ProducePeople;

        consumeDictionary[ResourceType.Power] = ConsumePower;
        consumeDictionary[ResourceType.Materials] = ConsumeMaterial;
        consumeDictionary[ResourceType.Food] = ConsumeFood;
        consumeDictionary[ResourceType.People] = ConsumePeople;
    }

    private float? getProduceValue(GameObject obj, ResourceType type)
    {
        var resourceComponent = obj.GetComponent<ResourceGenerator>();
        if (!resourceComponent) return null;
        var index = resourceComponent.resourceGeneration.resourceTypes.IndexOf(type);
        float? value = null;
        if (index >= 0)
        {
            value = resourceComponent.resourceGeneration.resourceQuantities[index];
        }

        return value;
    }

    private float? getConsumeValue(GameObject obj, ResourceType type)
    {
        var resourceComponent = obj.GetComponent<ResourceConsumer>();
        if (!resourceComponent) return null;
        var index = resourceComponent.resourceConsumption.resourceTypes.IndexOf(type);
        float? value = null;
        if (index >= 0)
        {
            value = resourceComponent.resourceConsumption.resourceQuantities[index];
        }

        return value;
    }

    private void setOrHideText(Text text, float? value, ResourceType type)
    {
        if (value == null)
        {
            text.enabled = false;
        }
        else
        {
            text.enabled = true;
            text.text = string.Format("{0} " + type, value.Value);
        }
    }

    private void setTextBasedOnValue(Placeable building, ResourceType type, bool produce = true)
    {
        float? value = produce? getProduceValue(building.gameObject, type) : getConsumeValue(building.gameObject, type);
        var text = produce ? produceDictionary[type] : consumeDictionary[type];

        Debug.Log(text.gameObject.name);
        Debug.Log(value);
        
        setOrHideText(text, value, type);
    }

    public void selectBuilding(Placeable prefabPlaceable)
    {
        var building = prefabPlaceable;
        
        Debug.Log("Selecting building " + building.placeableName);
        BuildingName.text = building.placeableName;
        
        foreach (var produceType in produceDictionary.Keys)
        {
            setTextBasedOnValue(building, produceType, true);
        }
        
        foreach (var consumeType in consumeDictionary.Keys)
        {
            setTextBasedOnValue(building, consumeType, false);
        }
        
    }
}
