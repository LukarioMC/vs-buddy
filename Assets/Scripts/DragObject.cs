using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 offset;
    private float zCoordinate;

    void OnMouseDown()
    {
        // Capture the object's Z coordinate in screen space
        zCoordinate = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // Calculate the offset between the mouse position and the object's center
        offset = gameObject.transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        // Continuously update the object's position to follow the mouse
        transform.position = GetMouseWorldPos() + offset;
    }

    private Vector3 GetMouseWorldPos()
    {
        // Get mouse position from screen point
        Vector3 mousePoint = Input.mousePosition;

        // Set the Z coordinate to the object's original Z coordinate in screen space
        mousePoint.z = zCoordinate;

        // Convert screen point to world point
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
