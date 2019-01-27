using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Placeable : MonoBehaviour, IPointerDownHandler
{
    public Vector2 centerPoint;

    public List<Vector2> localConnectionPoints = new List<Vector2>();
    public GameObject connectionPointPrefab;

    private Connection?[] connections;
    private GameObject[] connectionPointObjects;

    private bool shouldConnect = false;
    
    public bool isGhosted = false;

    private SpriteRenderer spriteRenderer;

    public Placeable()
    {
        
    }

    public List<Placeable> connected
    {
        get
        {
            var results = new List<Placeable>(connections.Length);
            foreach (var connection in connections)
            {
                if (connection.HasValue)
                {
                    results.Add(connection.Value.Placeable);
                }
            }

            return results;
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        List<Vector2> newConnectionPoints = new List<Vector2>();
        Vector3 size = GetComponent<SpriteRenderer>().sprite.bounds.size;
        foreach (var point in localConnectionPoints)
        {
            newConnectionPoints.Add(new Vector2(point.x * size.x, point.y * size.y));
        }

        localConnectionPoints = newConnectionPoints;
        centerPoint = new Vector2(centerPoint.x * size.x, centerPoint.y * size.y);
        connections = new Connection?[localConnectionPoints.Count];
        connectionPointObjects = new GameObject[localConnectionPoints.Count];
        
        var connectionPoints = worldConnectionPoints;
        for(var i = 0; i < connectionPoints.Count; i++) 
        {
            connectionPointObjects[i] = Instantiate(connectionPointPrefab, connectionPoints[i], Quaternion.identity, transform);
        }
        //Create a center point
        //Instantiate(connectionPointPrefab, worldCenterPoint, Quaternion.identity, transform);
    }

    void Update()
    {
        if (shouldConnect)
        {
            findAndConnect();
        }
    }

    public List<Vector3> worldConnectionPoints
    {
        get
        {
            return localConnectionPoints.ConvertAll(localPoint =>
                transform.localToWorldMatrix.MultiplyPoint(localPoint));
        }
    }

    public Vector3 worldCenterPoint
    {
        get { return transform.localToWorldMatrix.MultiplyPoint(centerPoint); }
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

    public void connect()
    {
        shouldConnect = true;
    }

    private void findAndConnect()
    {
        shouldConnect = false;
        
        var collider = GetComponent<Collider2D>();
        var candidates = new Collider2D[10];
        var filter = new ContactFilter2D();
        filter.useTriggers = true;
        var numCandidates = collider.OverlapCollider(filter, candidates);
        
        Debug.Log("Found " + numCandidates + " candidates");

        // Distance = 1 + distance to make sure it's > 1
        var maxConnectDistance = 0.2;
        var connectThreshold = maxConnectDistance * maxConnectDistance;

        for (var i = 0; i < numCandidates; i++)
        {
            var otherCollider = candidates[i];
            var otherPlaceable = otherCollider.gameObject.GetComponent<Placeable>();

            if (otherPlaceable == null)
            {
                //Debug.Log("Detected non-placeable collision with " + otherCollider.gameObject.name);
                continue;
            }

            var ourPoints = worldConnectionPoints;
            var theirPoints = otherPlaceable.worldConnectionPoints;

            for(int ourPointIndex = 0; ourPointIndex < ourPoints.Count; ourPointIndex++)
            {
                for(int theirPointIndex = 0; theirPointIndex < theirPoints.Count; theirPointIndex++)
                {
                    var ourPoint = ourPoints[ourPointIndex];
                    var theirPoint = theirPoints[theirPointIndex];
                    
                    var connectDistance = (theirPoint - ourPoint).sqrMagnitude;
                    Debug.Log("Distance is " + connectDistance);
                    if (connectDistance < connectThreshold)
                    {
                        attemptConnection(ourPointIndex, otherPlaceable, theirPointIndex);
                    }
                }
            }
        }
    }

    private void attemptConnection(int ourPointIndex, Placeable otherPlaceable, int theirPointIndex)
    {
        Debug.Log("Attempting connection");
        if (canConnectAtPoint(ourPointIndex) && otherPlaceable.canConnectAtPoint(theirPointIndex))
        {
            makeConnection(ourPointIndex, otherPlaceable, theirPointIndex);
        }
    }

    private void makeConnection(int ourPointIndex, Placeable otherPlaceable, int theirPointIndex)
    {
        var connectionSuccess = otherPlaceable.receiveConnection(theirPointIndex, this, ourPointIndex);
        if (connectionSuccess)
        {
            receiveConnection(ourPointIndex, otherPlaceable, theirPointIndex);
        }
    }

    private bool receiveConnection(int ourPointIndex, Placeable otherPlaceable, int theirPointIndex)
    {
        connections[ourPointIndex] = new Connection(otherPlaceable, theirPointIndex);
        connectionPointObjects[ourPointIndex].SetActive(false);
        return true;
    }

    private bool canConnectAtPoint(int ourPointIndex)
    {
        return connections[ourPointIndex] == null;
    }

    public struct Connection
    {
       public Placeable Placeable;
       public int ConnectionPoint;

       public Connection(Placeable placeable, int connectionPoint)
       {
           Placeable = placeable;
           ConnectionPoint = connectionPoint;
       }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
