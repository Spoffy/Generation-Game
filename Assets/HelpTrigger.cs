using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HelpTrigger : MonoBehaviour, IPointerEnterHandler
{
    public Placeable buildingToHelpWith;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        var helpWindow = GameObject.FindWithTag("HelpWindow").GetComponent<HelpWindow>();
        helpWindow.selectBuilding(buildingToHelpWith);
    }
}
