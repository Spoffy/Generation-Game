using System;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public Camera raycastInputCamera;
    public GameObject shadowPlaceablePrefab;
    public GameObject placeablePrefab;

    public bool IsInPlacementMode = false;

    private GameObject __shadowPlaceableInternal;
    
    private int selectedPoint = 0;

    private GameObject getShadowPlaceable()
    {     
        if (__shadowPlaceableInternal == null)
        {
            __shadowPlaceableInternal = Instantiate(shadowPlaceablePrefab, Vector3.zero, Quaternion.identity);
        }
        return __shadowPlaceableInternal;
    }

    private bool isShadowPlaced()
    {
        return __shadowPlaceableInternal != null;
    }

    private void clearShadowPlaceable()
    {
        if (__shadowPlaceableInternal)
        {
            Destroy(__shadowPlaceableInternal);
        }

        __shadowPlaceableInternal = null;
    }

    private Tuple<Vector3, Vector3> closestPair(List<Vector3> ourPoints, List<Vector3> theirPoints)
    {
        var distance = float.MaxValue;
        Vector3 ourChosenPoint = ourPoints[0];
        Vector3 theirChosenPoint = theirPoints[0];
        foreach (var ourPoint in ourPoints)
        {
            foreach (var theirPoint in theirPoints)
            {
                var distanceSqr = (theirPoint - ourPoint).sqrMagnitude;
                if (distanceSqr < distance)
                {
                    distance = distanceSqr;
                    ourChosenPoint = ourPoint;
                    theirChosenPoint = theirPoint;
                }
            }
        }
        return new Tuple<Vector3, Vector3>(ourChosenPoint, theirChosenPoint);
    }

    private Tuple<Vector3, Vector3> getConnectionTransform(Transform source, int sourcePoint, Placeable target, int targetPoint)
    {
        //Debug.Log(gameObject.name + " hit a placeable");
        var ourPlaceable = placeablePrefab.GetComponent<Placeable>();
        var theirPoint = target.worldConnectionPoints[targetPoint];
        var theirCenterPoint = target.worldCenterPoint;
        var ourPoint = ourPlaceable.getConnectionPointInWorld(sourcePoint, source);
        var ourCenterPoint = ourPlaceable.getCenterPointInWorld(source);

        //Getting vectors from the center of the shape to the connection points
        //Rotating to set the vectors to be facing opposite
        var targetRotationVector = -(theirPoint - theirCenterPoint);
        var ourLocalVector = ourPoint - ourCenterPoint;
            
        //var rotationAngle = Vector3.SignedAngle(ourLocalVector, targetRotationVector, new Vector3(0, 0, 1));
        var rotation = Quaternion.FromToRotation(ourLocalVector, targetRotationVector);
        //var rotation = Quaternion.identity;

        var matrix = Matrix4x4.Rotate(rotation);

        //var rotatedOurPoint = matrix.MultiplyPoint(ourPoint);
        //var translation = theirPoint - rotatedOurPoint;
        var rotatedLocalVector = matrix.MultiplyVector(ourLocalVector);
        //We need to add the vectors to move to the center point, then to the connection point.
        var ourCenterOffset = ourCenterPoint - source.position;
        var targetPosition = theirPoint - (rotatedLocalVector + ourCenterOffset);
        
        return new Tuple<Vector3, Vector3>(rotation.eulerAngles, targetPosition);
        
    }

    private void OnTriggerStay2D(Collider2D theirCollider)
    {
        if (!IsInPlacementMode)
        {
            return;
        }

        var otherPlaceable = theirCollider.gameObject.GetComponent<Placeable>();
        if (otherPlaceable != null)
        {
            //Debug.Log(gameObject.name + " hit a placeable");
            var theirPointPair = otherPlaceable.closestPointTo(transform.position);
            var theirPointIndex = theirPointPair.Item2;
            
            var shadowPlaceable = getShadowPlaceable();

            var linearTransform = getConnectionTransform(shadowPlaceable.transform, selectedPoint, otherPlaceable,
                theirPointIndex);

            shadowPlaceable.transform.Rotate(linearTransform.Item1);
            shadowPlaceable.transform.position = linearTransform.Item2;
        }
    }

    private void OnTriggerExit2D(Collider2D theirCollider)
    {
        clearShadowPlaceable();
    }

    public void place()
    {
        IsInPlacementMode = false;
        if (isShadowPlaced())
        {
            var shadowPlaceable = getShadowPlaceable();
            Instantiate(placeablePrefab, shadowPlaceable.transform.position, shadowPlaceable.transform.rotation);
        }
    }
}