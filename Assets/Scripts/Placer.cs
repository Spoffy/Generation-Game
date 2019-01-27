using System;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public Camera raycastInputCamera;
    public GameObject placeablePrefab;

    public bool IsInPlacementMode = false;

    private GameObject __shadowPlaceableInternal;
    
    private int selectedPoint = 0;
    
    private List<Placeable> collidingPlaceables = new List<Placeable>();

    private GameObject getShadowPlaceable()
    {     
        if (__shadowPlaceableInternal == null)
        {
            __shadowPlaceableInternal = Instantiate(placeablePrefab, Vector3.zero, Quaternion.identity);
            __shadowPlaceableInternal.GetComponent<Placeable>().isGhosted = true;
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
            var cached = __shadowPlaceableInternal;
            __shadowPlaceableInternal = null;
            Destroy(cached);
        }

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

    private Tuple<Vector3, Vector3> getConnectionTransform(Transform source, int sourcePoint, ConnectionPoint targetPoint)
    {
        //Debug.Log(gameObject.name + " hit a placeable");
        var ourPlaceable = placeablePrefab.GetComponent<Placeable>();
        //var theirPoint = target.worldConnectionPoints[targetPoint];
        var theirPoint = new Vector3(targetPoint.Point.x, targetPoint.Point.y, 0);
        var theirCenterPoint = targetPoint.Owner.worldCenterPoint;
        
        var ourPoint = ourPlaceable.getConnectionPointInWorld(sourcePoint, source);
        var ourCenterPoint = ourPlaceable.getCenterPointInWorld(source);
        
        var transformToShapeCenterVector = ourCenterPoint - source.position;

        //Getting vectors from the center of the shape to the connection points
        //Rotating to set the vectors to be facing opposite
        var targetRotationVector = -(theirPoint - theirCenterPoint);
        var shapeCenterToPointVector = ourPoint - ourCenterPoint;
        var transformToPointVector = transformToShapeCenterVector + shapeCenterToPointVector;
            
        //var rotationAngle = Vector3.SignedAngle(ourLocalVector, targetRotationVector, new Vector3(0, 0, 1));
        var rotation = Quaternion.FromToRotation( shapeCenterToPointVector, targetRotationVector);
        //var rotation = Quaternion.identity;

        var matrix = Matrix4x4.Rotate(Quaternion.identity);//rotation);

        //var rotatedOurPoint = matrix.MultiplyPoint(ourPoint);
        //var translation = theirPoint - rotatedOurPoint;
        var offsetFromTargetPointVector = matrix.MultiplyVector( transformToPointVector );
        //We need to add the vectors to move to the center point, then to the connection point.
        var targetPosition = theirPoint;
        
        return new Tuple<Vector3, Vector3>(new Vector3(0,0,0), targetPosition);//rotation.eulerAngles, targetPosition);
        
    }


    private ConnectionPoint closestConnectionPoint()
    {
        var distance = float.MaxValue;
        Placeable chosenPointOwner = null;
        int chosenPointIndex = 0;
        Vector2 chosenPoint = Vector2.zero;
        
        foreach (Placeable placeable in collidingPlaceables)
        {
            var theirPoints = placeable.worldConnectionPoints;
            for (var i = 0; i < theirPoints.Count; i++)
        {
            var theirPoint = theirPoints[i];
            var distanceSqr = (transform.position - theirPoint).sqrMagnitude;
            if (distanceSqr < distance)
            {
                distance = distanceSqr;
                chosenPointIndex = i;
                chosenPointOwner = placeable;
                chosenPoint = theirPoint;
            }
        }
            
        }


        return new ConnectionPoint(chosenPointOwner, chosenPointIndex, chosenPoint);
    }
    
    private void OnTriggerEnter2D(Collider2D theirCollider)
    {
        var placeable = theirCollider.GetComponent<Placeable>();
        if (placeable != null && theirCollider.gameObject != gameObject && !placeable.isGhosted)
        {
            collidingPlaceables.Add(placeable);
        }
    }
    
    void Update()
    {
        if (collidingPlaceables.Count <= 0 || !IsInPlacementMode)
        {
            return;
        }

        var targetConnectionPoint = closestConnectionPoint();
        var shadowPlaceable = getShadowPlaceable();
        var linearTransform = getConnectionTransform(shadowPlaceable.transform, selectedPoint, targetConnectionPoint);
        
        shadowPlaceable.transform.Rotate(linearTransform.Item1);
        shadowPlaceable.transform.position = linearTransform.Item2;
    }
    /*
    private void OnTriggerStay2D(Collider2D theirCollider)
    {
        var otherPlaceable = theirCollider.gameObject.GetComponent<Placeable>();
        if (otherPlaceable != null)
        {
            //Debug.Log(gameObject.name + " hit a placeable");
            var theirPointPair = otherPlaceable.closestPointTo(transform.position);
            var theirPointIndex = theirPointPair.Item2;
            Debug.Log(theirPointIndex);
            
            var shadowPlaceable = getShadowPlaceable();

            var linearTransform = getConnectionTransform(shadowPlaceable.transform, selectedPoint, otherPlaceable,
                theirPointIndex);

            shadowPlaceable.transform.Rotate(linearTransform.Item1);
            shadowPlaceable.transform.position = linearTransform.Item2;
        }
    }*/

    private void OnTriggerExit2D(Collider2D theirCollider)
    {
        var placeable = theirCollider.GetComponent<Placeable>();
        if (placeable != null)
        {
            collidingPlaceables.Remove(placeable);
        }

        if (collidingPlaceables.Count <= 0)
        {
            clearShadowPlaceable();
        }
    }

    public void place()
    {
        IsInPlacementMode = false;
        if (isShadowPlaced())
        {
            var shadowPlaceable = getShadowPlaceable();
            __shadowPlaceableInternal = null;
            var placedObject = shadowPlaceable.GetComponent<Placeable>();
            placedObject.isGhosted = false;
            placedObject.connect();
        }
    }

    private struct ConnectionPoint
    {
        public Placeable Owner;
        public int Index;
        public Vector2 Point;

        public ConnectionPoint(Placeable owner, int index, Vector2 point)
        {
            Owner = owner;
            Index = index;
            Point = point;
        }
    }
}