﻿using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementButton : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Placer placer;
    
    // Start is called before the first frame update
    void Start()
    {
        placer = GameObject.Find("Crosshair").GetComponent<Placer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        placer.IsInPlacementMode = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        placer.place();
        placer.IsInPlacementMode = false;
    }
}