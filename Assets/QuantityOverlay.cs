using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ResourceStorage))]
public class QuantityOverlay : MonoBehaviour
{
    public ResourceType resourceType;
    private ResourceStorage storage;
    // Start is called before the first frame update
    void Start()
    {
        storage = GetComponent<ResourceStorage>();
    }

    // Update is called once per frame
    void Update()
    {
        var canvas = GetComponentInChildren<Canvas>();
        canvas.gameObject.transform.rotation = Quaternion.identity;
        var textOutput = GetComponentInChildren<Text>();
        if (textOutput)
        {
            var quantity = storage.resources[resourceType];
            textOutput.text = string.Format("{0:0}", quantity);
        }   
    }
}
