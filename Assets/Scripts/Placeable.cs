﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Placeable : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    public Camera raycastInputCamera;
    [FormerlySerializedAs("connectionPoints")] 
    public List<Vector2> localConnectionPoints = new List<Vector2>();
    public GameObject connectionPointPrefab;

    private bool isDragged = false;

    
    void Start()
    {
        foreach (var connectionPoint in worldConnectionPoints)
        {
            Instantiate(connectionPointPrefab, connectionPoint, Quaternion.identity, transform);
        } 
    }

    public List<Vector3> worldConnectionPoints
    {
        get
        {
            return localConnectionPoints.ConvertAll<Vector3>(localPoint => transform.localToWorldMatrix.MultiplyPoint(localPoint));
        }
    }

    public Vector3 getConnectionPointInWorld(int connectionPointIndex, Transform theirTransform)
    {
        return theirTransform.localToWorldMatrix.MultiplyPoint(localConnectionPoints[connectionPointIndex]);
    }

    public Tuple<Vector3,int> closestPointTo(Vector3 point)
    {
        var distance = float.MaxValue;
        var ourPoints = worldConnectionPoints;
        int chosenPointIndex = 0;
        for(var i = 0; i < ourPoints.Count; i++)
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

    public void OnDrag(PointerEventData eventData)
    {
        isDragged = eventData.dragging;
        gameObject.GetComponent<Rigidbody2D>().position = raycastInputCamera.ScreenToWorldPoint(eventData.position);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    private void OnMouseUp()
    {
        isDragged = false;
    }
}