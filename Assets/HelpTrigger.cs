using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HelpTrigger : MonoBehaviour, IPointerEnterHandler
{
    public HelpWindow helpWindow;
    public Placeable buildingToHelpWith;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        helpWindow.selectBuilding(buildingToHelpWith);
    }
}
