using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCamera : MonoBehaviour
{
    public EventSystem eventSystem;
    public Camera camera;

    private PointerEventData pointerEventData;
    private Vector3? startingCameraPosition;
    private Vector3? startingScreenPosition;
    private Vector3? startingWorldPosition;
    private Vector3 cameraDelta;
    private Vector3 worldDelta;
    private Vector3? deltaVector;
    
    void Update()
    {
        //Check if the left Mouse button is clicked
        if (Input.GetMouseButtonDown(1))
        {
            if (startingScreenPosition == null)
            {
                //Set up the new Pointer Event
                pointerEventData = new PointerEventData(eventSystem);
                //Set the Pointer Event Position to that of the mouse position
                pointerEventData.position = Input.mousePosition;

                //Create a list of Raycast Results
                List<RaycastResult> results = new List<RaycastResult>();

                //Raycast using theAll Graphics Raycaster and mouse click position
                eventSystem.RaycastAll(pointerEventData, results);

                if (results.Count <= 0)
                {
                    beginDrag(Input.mousePosition);
                }
            }
        }

        if (Input.GetMouseButtonUp(1) && startingScreenPosition != null)
        {
            endDrag();
        }

        if(startingScreenPosition != null && deltaVector == null)
        {
            if (Input.mousePosition != startingScreenPosition)
            {
                var changeInScreen = Input.mousePosition - startingScreenPosition.Value;
                var changeInWorld = camera.ScreenToWorldPoint(Input.mousePosition) - startingWorldPosition.Value;
                float x = -changeInWorld.x / changeInScreen.x;
                var y = -changeInWorld.y / changeInScreen.y;
                var z = 0;
                x = float.IsNaN(x) ? 0 : x;
                y = float.IsNaN(y) ? 0 : y;
                z = float.IsNaN(z) ? 0 : z;
                deltaVector = new Vector3(x, y, z);
            }
        }
        else if (deltaVector != null && startingScreenPosition != null)
        {
            Debug.Log(deltaVector); 
            var changeInScreen = Input.mousePosition - startingScreenPosition.Value;
            var changeInWorld = Vector3.Scale(changeInScreen, deltaVector.Value);
            camera.transform.position = startingCameraPosition.Value + changeInWorld;
        }
    }

    private void endDrag()
    {
        startingWorldPosition = null;
        startingScreenPosition = null;
        startingCameraPosition = null;
        deltaVector = null;
    }

    private void beginDrag(Vector3 mousePosition)
    {
        startingCameraPosition = camera.transform.position;
        startingScreenPosition = mousePosition;
        startingWorldPosition = camera.ScreenToWorldPoint(mousePosition);
    }
}
