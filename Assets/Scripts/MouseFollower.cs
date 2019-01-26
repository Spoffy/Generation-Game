using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(worldPoint.x, worldPoint.y, 0);
    }
}
