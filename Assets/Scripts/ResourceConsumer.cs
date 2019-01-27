using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
public class ResourceConsumer : MonoBehaviour, ITickable
{
    public const int CONSUMER_TICK_PRIORITY = 8;
    public bool functioning = true;
    public bool fed = false;
    public ResourceDictionary resourceConsumption = new ResourceDictionary();
    public ResourceDictionary shortages = new ResourceDictionary();

    public bool hasGeneralAlert;
    public GameObject generalAlert;
    public bool hasPowerAlert;
    public GameObject powerAlert;
    public bool hasMaterialAlert;
    public GameObject materialAlert;
    public bool hasFoodAlert;
    public GameObject foodAlert;
    public bool hasPeopleAlert;
    public GameObject peopleAlert;

    public static bool isFunctioning(GameObject obj)
    {
        var resourceConsumer = obj.GetComponent<ResourceConsumer>();
        if (!resourceConsumer)
        {
            return true; 
        }

        return resourceConsumer.functioning;
    }
    
    void Start()
    {
        resourceConsumption.populate();
        shortages.populate();
        Ticker.FindTicker().Register(this, CONSUMER_TICK_PRIORITY);
    }

    void Update()
    {
        if (hasGeneralAlert && functioning == false)
        {
            generalAlert.SetActive(true);
        }
        else if(hasGeneralAlert)
        {
            generalAlert.SetActive(false);
        }
        
        if (hasPowerAlert && shortages[ResourceType.Power] > 0)
        {
            powerAlert.SetActive(true);
        }
        else if(hasPowerAlert)
        {
            
            powerAlert.SetActive(false);
        }
        
        if (hasMaterialAlert && shortages[ResourceType.Materials] > 0)
        {
            materialAlert.SetActive(true);
        }
        else if (hasMaterialAlert)
        {
            
            materialAlert.SetActive(false);
        }
                
        if (hasFoodAlert && shortages[ResourceType.Food] > 0)
        {
            foodAlert.SetActive(true);
        }
        else if(hasFoodAlert)
        {
            
            foodAlert.SetActive(false);
        }
        if (hasPeopleAlert && shortages[ResourceType.People] > 0)
        {
            peopleAlert.SetActive(true);
        }
        else if (hasPeopleAlert)
        {
            
            peopleAlert.SetActive(false);
        }
    }

    public bool Consume()
    {
        fed = true;
        shortages.zero(); 
        var storage = GetComponent<ResourceStorage>();
        foreach (var resourcePair in resourceConsumption)
        {
            var amountToConsume = resourcePair.Value;
            var amountInStorage = storage.resources[resourcePair.Key];
            if (amountInStorage < amountToConsume)
            {
                fed = false;
                shortages[resourcePair.Key] = 1;
            }
        }

        return fed;
    }
    
    public void Tick()
    {
        Consume();
        if (!fed)
        {
            Debug.Log("Oh no, " + gameObject.name + " isn't fed!");
            functioning = false;
        }
        else
        {
            functioning = true;
        }
    }
}
