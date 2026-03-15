using UnityEngine;

public class DragObject2D : MonoBehaviour
{
    public GameObject dragObjectPrefab;
    public float updateInterval = 1.0f; // Time interval for updating the position
    public GameObject bonziObject;
    public BoxCollider2D[] wallColliders;
    public float overlapThreshold = 0.1f;
    
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

    void OnMouseUp()
    {
        // Destroy the drag object when the mouse button is released
        if (dragObject != null)
        {
            Destroy(dragObject);
            dragObject = null;
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

        if (bonziObject != null)
        {            
            Collider2D pc = bonziObject.GetComponentInChildren<Collider2D>();

            foreach (Collider2D collider in wallColliders)
            {
                // 1. Get the distance data between the two colliders
                ColliderDistance2D dist = pc.Distance(collider);

                // 2. Check if they are actually overlapping
                // In Unity, a negative distance means they are overlapping
                if (dist.isOverlapped && Mathf.Abs(dist.distance) > overlapThreshold)
                {
                    // 3. Force the transform over by the exact overlap amount
                    // 'distance' is negative here, so we multiply by the normal to push away
                    bonziObject.transform.position -= (Vector3)(dist.normal * Mathf.Abs(dist.distance));
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (dragObject != null)
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

            dragObject.transform.rotation = Quaternion.Euler(0, 0, angleDeg);
            Rigidbody2D rb = dragObject.GetComponent<Rigidbody2D>();
            rb.MovePosition(newPosition);
        }
    }
}
