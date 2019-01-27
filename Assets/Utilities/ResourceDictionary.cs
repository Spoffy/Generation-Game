using System;
using System.Collections.Generic;
using System.Linq;
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

        foreach (var resourceType in (ResourceType[]) Enum.GetValues(typeof(ResourceType)))
        {
            if (!ContainsKey(resourceType))
            {
                Add(resourceType, 0);
            }
        }
    }

    public void zero()
    {
        var keys = new List<ResourceType>(Keys);
        foreach(var key in keys)
        {
            this[key] = 0;
        }
    }

}
