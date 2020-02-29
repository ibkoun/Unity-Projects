using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Behaviour script that enables mouse drag.
/// </summary>
public class MouseDragScript : MonoBehaviour
{
    private RaycastHit raycastHit;

    public Vector3 Raycast { get; set; }

    public Vector3 CameraPositionReference { get; set; } // Position of the camera on mouse click.

    public Quaternion CameraRotationReference { get; set; } // Rotation of the camera on mouse click.

    public Vector3 MousePositionReference { get; set; } // Raycast hit point on mouse click.

    public Vector3 MouseOffset { get; set; } // From the raycast hit point to object's center point.

    public Camera MainCamera { get; set; }

    public Transform CameraTransform { get; set; }

    public Transform ObjectTransform { get; set; }

    public bool ObjectSelected { get; set; }

    public bool MouseLeftClick { get; set; }

    public Image Reticle { get; set; }

    private void Start()
    {
        MainCamera = Camera.main;
        CameraTransform = MainCamera.transform.parent;
        ObjectTransform = transform;
        Reticle = FindObjectOfType<Canvas>().GetComponentsInChildren<Image>()[0];
    }

    private void Update()
    {
        // The object is not selected anymore if the mouse left button is released.
        if (!Input.GetMouseButton(0) && MouseLeftClick)
        {
            if (Reticle != null) Reticle.color = Color.white;
            MouseLeftClick = false;
            ObjectSelected = false;
        }
    }

    private void OnMouseOver()
    {
        bool mouseLeftButtonDown = Input.GetMouseButtonDown(0);
        bool mouseLeftButtonUp = Input.GetMouseButtonUp(0);

        if (Reticle != null) Reticle.color = Color.red; // Reticles turns red when the object is in range.
        if (mouseLeftButtonDown) // On mouse left click.
        {
            // Create a raycast from the main camera in the direction of it's transform forward vector.
            // TODO: Add a range for the raycast inside the chosen camera script.
            if (Physics.Raycast(CameraTransform.position, CameraTransform.forward, out raycastHit, Mathf.Infinity))
            {
                Raycast = raycastHit.distance * CameraTransform.forward;
                MousePositionReference = raycastHit.point;
                MouseOffset = MousePositionReference - ObjectTransform.position;
                CameraRotationReference = CameraTransform.rotation;
                CameraPositionReference = CameraTransform.position;

                MouseLeftClick = mouseLeftButtonDown;
                ObjectSelected = mouseLeftButtonDown;
            }
        }
        else if (mouseLeftButtonUp) // On mouse left button release.
        {
            MouseLeftClick = !mouseLeftButtonUp;
            ObjectSelected = !mouseLeftButtonUp;
        }
    }

    private void OnMouseExit()
    {
        // The object is still selected if the mouse left button is held.
        bool mouseLeftButton = Input.GetMouseButton(0);
        if (mouseLeftButton) ObjectSelected = mouseLeftButton;
        else if (Reticle != null) Reticle.color = Color.white;
    }

    // Return the rotation performed by the camera.
    public Quaternion CameraRotation()
    {
        Quaternion rotation = CameraTransform.rotation * Quaternion.Inverse(CameraRotationReference);
        CameraRotationReference = CameraTransform.rotation;
        Raycast = rotation * Raycast;
        return rotation;
    }

    // Return the displacement of the selected object on mouse drag.
    public Vector3 ObjectDisplacement()
    {
        return (CameraTransform.position + raycastHit.distance * CameraTransform.forward - MouseOffset) - ObjectTransform.position;
    }

    // Return the raycast hit point based on the position of the mouse.
    public Vector3 CameraRaycastHitPoint()
    {
        return CameraTransform.position + raycastHit.distance * CameraTransform.forward;
    }
}

/* References:
 * https://letsgeekblog.wordpress.com/2017/03/11/drag-object-script-in-unity3d/
 */
