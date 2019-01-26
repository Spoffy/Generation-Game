using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Placeable : MonoBehaviour
{
    public Camera raycastInputCamera;

    public Vector2 centerPoint;

    public List<Vector2> localConnectionPoints = new List<Vector2>();
    public GameObject connectionPointPrefab;

    void Start()
    {
        List<Vector2> newConnectionPoints = new List<Vector2>();
        Vector3 size = GetComponent<SpriteRenderer>().sprite.bounds.size;
        foreach (var point in localConnectionPoints)
        {
            newConnectionPoints.Add(new Vector2(point.x * size.x, point.y * size.y));
        }

        localConnectionPoints = newConnectionPoints;
        centerPoint = new Vector2(centerPoint.x * size.x, centerPoint.y * size.y);

        Debug.Log(GetComponent<SpriteRenderer>().bounds);
        foreach (var connectionPoint in worldConnectionPoints)
        {
            Instantiate(connectionPointPrefab, connectionPoint, Quaternion.identity, transform);
        }
    }

    public List<Vector3> worldConnectionPoints
    {
        get
        {
            return localConnectionPoints.ConvertAll<Vector3>(localPoint =>
                transform.localToWorldMatrix.MultiplyPoint(localPoint));
        }
    }

    public Vector3 worldCenterPoint
    {
        get { return transform.localToWorldMatrix.MultiplyPoint(centerPoint); }
    }

    public Vector3 getConnectionPointInWorld(int connectionPointIndex, Transform theirTransform)
    {
        return theirTransform.localToWorldMatrix.MultiplyPoint(localConnectionPoints[connectionPointIndex]);
    }

    public Vector3 getCenterPointInWorld(Transform theirTransform)
    {
        return theirTransform.localToWorldMatrix.MultiplyPoint(centerPoint);
    }

    public Tuple<Vector3, int> closestPointTo(Vector3 point)
    {
        var distance = float.MaxValue;
        var ourPoints = worldConnectionPoints;
        int chosenPointIndex = 0;
        for (var i = 0; i < ourPoints.Count; i++)
        {
            var ourPoint = ourPoints[i];
            var distanceSqr = (point - ourPoint).sqrMagnitude;
            if (distanceSqr < distance)
            {
                distance = distanceSqr;
                chosenPointIndex = i;
            }
        }

        return new Tuple<Vector3, int>(ourPoints[chosenPointIndex], chosenPointIndex);
    }
}
