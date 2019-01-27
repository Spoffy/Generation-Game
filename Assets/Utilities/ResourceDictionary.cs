using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceDictionary : Dictionary<ResourceType, float>
{
    [SerializeField]
    public List<ResourceType> resourceTypes = new List<ResourceType>();

    [SerializeField]
    public List<float> resourceQuantities = new List<float>();

    public void populate()
    {
        if (resourceTypes.Count != resourceQuantities.Count)
        {
            Debug.LogError("Warning: Mismatched resource type and quantity");
            return;
        }

        for (var i = 0; i < resourceTypes.Count; i++)
        {
            Add(resourceTypes[i], resourceQuantities[i]);
        }
    }

}
