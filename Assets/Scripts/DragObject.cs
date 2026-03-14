using UnityEngine;

public class DragObject2D : MonoBehaviour
{
    public GameObject dragObjectPrefab;
    public float updateInterval = 1.0f; // Time interval for updating the position
    private GameObject dragObject;
    private Vector3 lastPosition;
    private float timeSinceLastUpdate = 0f;
    
    void OnMouseDown()
    {
        // Calculate the offset between the mouse position and the object's position
        lastPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        dragObject = Instantiate(dragObjectPrefab, lastPosition, Quaternion.identity);
        timeSinceLastUpdate = 0f;
    }

    void OnMouseDrag()
    {
        // Continuously update the object's position with the mouse position plus the offset
        Vector3 newPosition =  Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Vector3 direction = newPosition - lastPosition;
        float angleRad = Mathf.Atan2(direction.y, direction.x);
        float angleDeg = angleRad * Mathf.Rad2Deg;

        SpriteRenderer renderer = dragObject.GetComponentInChildren<SpriteRenderer>();
        if(direction.x < 0)
        {
            renderer.flipY = true;
            //dragObject.transform.localScale = new Vector3(-1, 1, 1); // Flip the object horizontally
            angleDeg += 180; // Adjust the angle for the flipped object
        }
        else
        {
            renderer.flipY = false;
            //dragObject.transform.localScale = new Vector3(1, 1, 1); // Normal scale
        }

        dragObject.transform.position = newPosition;
        dragObject.transform.rotation = Quaternion.Euler(0, 0, angleDeg);
        //transform.position = Camera.main.ScreenToWorldPoint(newPosition) + offset;
    }

    void OnMouseUp()
    {
        // Destroy the drag object when the mouse button is released
        if (dragObject != null)
        {
            Destroy(dragObject);
        }
    }

    void Update()
    {
        if (dragObject != null)
        {
            timeSinceLastUpdate += Time.deltaTime;

            if (timeSinceLastUpdate >= updateInterval)
            {
                // Update the position of the drag object
                lastPosition = dragObject.transform.position;
                timeSinceLastUpdate = 0f; // Reset the timer
            }
        }
    }
}
