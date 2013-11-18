using UnityEngine;
using System.Collections;

/// <summary>
/// Igor camera
/// </summary>
public class IgorCamera : MonoBehaviour
{
    /// <summary>
    /// Camera will not check objects with this tag for coming closer.
    /// </summary>
    public string dontCheckThisTag;

    /// <summary>
    /// The target to follow.
    /// </summary>
    public Transform target;

    /// <summary>
    /// Which distance should we keep from the target.
    /// </summary>
    public float distance = 20f;

    /// <summary>
    /// The actual distance we have from our enemy.
    /// </summary>
    private float currentDistance;

    /// <summary>
    /// The minimal distance from the target.
    /// </summary>
    public float minimalDistance = 10f;

    /// <summary>
    /// The maximal distance from the target.
    /// </summary>
    public float maximalDistance = 30f;

    /// <summary>
    /// How fast should the camera rotate?
    /// </summary>
    public float rotationSpeed = 8f;

    /// <summary>
    /// The maximal angle for the vertical rotation (0 = directly behind, 90 = directly above, -90 = directly below).
    /// </summary>
    public float minimalAngle = 0f;

    /// <summary>
    /// The maximal angle for the vertical rotation (0 = directly behind, 90 = directly above, -90 = directly below).
    /// </summary>
    public float maximalAngle = 60f;

    /// <summary>
    /// The rotation x and y components.
    /// </summary>
    private float x, y;

    /// <summary>
    /// crosshair transform
    /// </summary>
    private Transform crosshair;

    /// <summary>
    /// Set the camera initially to the correct position / rotation.
    /// </summary>
    private void Start()
    {
        transform.position = new Vector3(0, 0, -distance) + target.position;
        transform.LookAt(target);
        x += transform.eulerAngles.y;
        y += transform.eulerAngles.x;

        crosshair = transform.FindChild("Crosshair");
    }

    /// <summary>
    /// This is called after all other Update-Methods.
    /// </summary>
    private void LateUpdate()
    {
        if (!Game.IsPaused && !Game.isMenuOpen)
        {
            // Zoom in/out
            distance -= Input.GetAxis("Mouse ScrollWheel") * distance;
            distance = Mathf.Clamp(distance, minimalDistance, maximalDistance);

            // Reset current distance
            currentDistance = distance;

            // Translation
            x += Input.GetAxis("Mouse X") * rotationSpeed;
            y -= Input.GetAxis("Mouse Y") * rotationSpeed;

            // Cut off too much vertical movement
            y = Mathf.Clamp(y, minimalAngle, maximalAngle);

            // Rotate and position the object correctly
            Quaternion rotation = Quaternion.Euler(y, x, 0);
            transform.position = rotation * new Vector3(0, 0, -currentDistance) + target.position;
            transform.rotation = rotation;

            // Rotate the target the same angle
            target.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);

            // Raycast from Igor towards the camera to check for objects in the line of sight
            RaycastHit[] hits = Physics.RaycastAll(target.position, -transform.forward, currentDistance + 3f);
            foreach (RaycastHit hit in hits)
            {
                // Don't check collisions with Igor himself
                if (hit.collider.gameObject == Game.GetIgor())
                {
                    continue;
                }

                // Don't check collisions with specific tag
                if (hit.collider.tag.Contains(dontCheckThisTag))
                {
                    continue;
                }

                // If there is something between camera and Igor, move camera closer to Igor
                if ((hit.point - target.position).magnitude < currentDistance)
                {
                    currentDistance = (hit.point - target.position).magnitude - 1.5f;
                    transform.position = rotation * new Vector3(0, 0, -currentDistance) + target.position;
                }
            }

            // Check distance from camera to Igor -- if we are too close hide Igor
            
            Igor igor = Game.GetIgor().GetComponent<Igor>();
            if (currentDistance < 2f)
            {
                igor.Hidden = true;
            }
            else
            {
                igor.Hidden = false;
            }

            // Crosshair
            if (Game.InGame)
            {
                crosshair.gameObject.SetActive(true);
                RaycastHit hitInfo;
                if (Physics.Raycast(igor.collider.bounds.center, new Vector3(transform.forward.x, 0, transform.forward.z), out hitInfo))
                {
                    if (!hitInfo.collider.tag.Contains("Projectile") && !hitInfo.collider.tag.Contains("Player"))
                    {
                        crosshair.position = hitInfo.point;
                        Vector3 dist = transform.position - crosshair.position;
                        crosshair.localScale = new Vector3(dist.magnitude, dist.magnitude, dist.magnitude) * 0.02f;
                    }
                }
            }
            else
            {
                crosshair.gameObject.SetActive(false);
            }
        }
    }
}
