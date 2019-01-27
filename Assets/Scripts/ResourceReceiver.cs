using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
public abstract class ResourceReceiver : MonoBehaviour
{
   public abstract void flowFrom(ResourceType resourceType, float quantity, int sourceFlowIteration);
}
