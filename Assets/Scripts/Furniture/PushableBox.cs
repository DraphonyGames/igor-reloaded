using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Pushable box.
/// 
/// Needs a collider and has to be a cube. Needs the tag "Usable". Should only rotated horizontally, otherwise weird things will happen...
/// </summary>
public class PushableBox : MonoBehaviour, IUsable
{
    /// <summary>
    /// Amount the box should be used at each push/pull step
    /// </summary>
    public float moveSpeed = 3;

    /// <summary>
    /// Set when falling down to prevent pulling/pushing
    /// </summary>
    public bool FallingDown
    {
        get { return _fallingDown; }
    }

    /// <summary>
    /// Old position, prior to moving
    /// </summary>
    private Vector3 oldPosition;

    /// <summary>
    /// Set when falling down to prevent pulling/pushing
    /// </summary>
    private bool _fallingDown = false;

    /// <summary>
    /// Defines how fast the box should fall
    /// </summary>
    public float fallSpeed = 8;

    /// <summary>
    /// Our real size, read only
    /// </summary>
    public Vector3 RealSize
    {
        get { return _realSize; }
    }

    /// <summary>
    /// Our real size: internal variable
    /// </summary>
    private Vector3 _realSize;

    /// <summary>
    /// False, if we only have an estimate
    /// </summary>
    private bool gotRealRealSize = false;

    /// <summary>
    /// Since Unity is sometimes buggy (GetComponent&lt;MeshFilter&gt;().mesh throws an exception, although everything is not null...)
    /// we may have to do it more then once... 
    /// </summary>
    private void CalculateRealSize()
    {
        if (!gotRealRealSize)
        {
            // Workaround for Unity.. sometimes (pretty rare) it crashes, even if it absolutely shouldn't...
            try
            {
                // Use an estimate, until we have real data.. damn you, Unity..
                _realSize = new Vector3(collider.bounds.size.x, collider.bounds.size.y, collider.bounds.size.z);

                MeshFilter meshFilter = GetComponent<MeshFilter>();

                if (meshFilter != null)
                {
                    Mesh mesh = GetComponent<MeshFilter>().mesh;
                    _realSize = new Vector3(mesh.bounds.size.x * transform.localScale.x, mesh.bounds.size.y * transform.localScale.y, mesh.bounds.size.z * transform.localScale.z);
                }
                else
                {
                    // maybe this is a skinned object
                    SkinnedMeshRenderer[] skinMeshs = GetComponentsInChildren<SkinnedMeshRenderer>();
                    float skinSizeX = -1;
                    float skinSizeY = -1;
                    float skinSizeZ = -1;

                    foreach (SkinnedMeshRenderer smr in skinMeshs)
                    {
                        if (skinSizeX < smr.localBounds.size.x)
                        {
                            skinSizeX = smr.localBounds.size.x;
                        }

                        if (skinSizeY < smr.localBounds.size.y)
                        {
                            skinSizeY = smr.localBounds.size.y;
                        }

                        if (skinSizeZ < smr.localBounds.size.z)
                        {
                            skinSizeZ = smr.localBounds.size.z;
                        }
                    }

                    _realSize = new Vector3(skinSizeX, skinSizeY, skinSizeZ);
                }

                gotRealRealSize = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("Unity had some problems getting the mesh out of a PushableBox... : " + e.Message);
            }
        }
    }

    /// <summary>
    /// save the current position
    /// </summary>
    public void SavePosition()
    {
        oldPosition = gameObject.transform.position;
    }

    /// <summary>
    /// Load the saved position
    /// </summary>
    public void LoadOldPosition()
    {
        gameObject.transform.position = oldPosition;
    }

    /// <summary>
    /// Called by Unity
    /// </summary>
    private void Start()
    {
        gameObject.tag = "Need Save,Usable";
        fallSpeed = Mathf.Abs(fallSpeed); // just to be sure..

        CalculateRealSize();
    }

    /// <summary>
    /// Returns if you are allowed to push the object
    /// </summary>
    /// <param name="igorGO">Igor game object</param>
    /// <param name="forward">moving forward?</param>
    /// <returns>Returns if you are allowed to push the object</returns>
    public bool PushingAllowed(GameObject igorGO, bool forward)
    {
        // No pulling + pushing when falling down
        if (_fallingDown)
        {
            return false;
        }

        Vector3 pushingDir = GetPushingDirection(igorGO.transform.position);
        Vector3 baseFront = gameObject.transform.position + (pushingDir * 0.45f * RealSize.x * (forward ? 1 : -1)); // 0.45 to start inside the object
        Vector3 baseIgor = (new Vector3(igorGO.transform.position.x, transform.position.y, igorGO.transform.position.z)) - (pushingDir * 0.45f * igorGO.GetComponent<Igor>().RealSize.z);

        Vector3 basevec = pushingDir * 0.5f * RealSize.x;
        Vector3 baseRight = Quaternion.AngleAxis(90, Vector3.up) * basevec * 0.8f; // tiny borders are ok
        Vector3 baseUp = Vector3.up * basevec.magnitude * 0.8f; // tiny borders are ok

        // Split, to allow other detection at igor/upper edge when moving backwards
        Vector3[] upperEdges;

        if (forward)
        {
            upperEdges = new Vector3[]
            {
                baseFront + baseRight + baseUp, 
                baseFront - baseRight + baseUp, 
            };
        }
        else
        {
            upperEdges = new Vector3[]
            {
                baseIgor + baseRight + baseUp, 
                baseIgor - baseRight + baseUp, 
            };
        }

        Vector3[] lowerEdges =
        {
            baseFront + baseRight - baseUp, 
            baseFront - baseRight - baseUp
        };

        RaycastHit hit;

        foreach (Vector3 v in upperEdges)
        {
            if (Physics.Raycast(v, pushingDir * (forward ? 1 : (-1)), out hit))
            {
                if (hit.distance < RealSize.x)
                {
                    return false;
                }
            }
        }

        foreach (Vector3 v in lowerEdges)
        {
            if (Physics.Raycast(v, pushingDir * (forward ? 1 : (-1)), out hit))
            {
                if (hit.distance < RealSize.x)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the direction, in which it will be pushed forward. Only north/west/south/east possible
    /// </summary>
    /// <param name="playerpos">Position of the player</param>
    /// <returns>Direction, in which it will be pushed forward</returns>
    public Vector3 GetPushingDirection(Vector3 playerpos)
    {
        Vector3 dir = playerpos - transform.position;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            return Vector3.left * Mathf.Sign(dir.x);
        }
        else
        {
            return Vector3.back * Mathf.Sign(dir.z);
        }
    }

    /// <summary>
    /// should be called when the object "obj" wants to use the trigger (via a usage action: IUsable)
    /// </summary>
    /// <param name="obj">Object which triggered it</param>
    public void OnUse(GameObject obj)
    {
        Igor igor = Game.GetIgor().GetComponent<Igor>();
        igor.InitiatePushing(this);
    }

    /// <summary>
    /// Called by Unity
    /// </summary>
    public void Update()
    {
        FallDown();
        CalculateRealSize(); // just to be sure...
    }

    /// <summary>
    /// Lets the manually box fall down
    /// </summary>
    public void FallDown()
    {
        float fallDistance = fallSpeed * Time.deltaTime;
        Vector3 basevec = transform.position + (Vector3.down * 0.49999f * RealSize.y); // 0.4 to start inside the object
        Vector3 baseRight = transform.rotation * Vector3.forward * RealSize.x * 0.5f * 0.98f;
        Vector3 baseFront = transform.rotation * Quaternion.AngleAxis(90, Vector3.up) * Vector3.forward * RealSize.x * 0.5f * 0.98f;
        
        // Split, to allow other detection at igor/upper edge when moving backwards
        Vector3[] edges =
        {
            basevec + baseRight + baseFront,
            basevec + baseRight - baseFront,
            basevec - baseRight + baseFront,
            basevec - baseRight - baseFront
        };

        RaycastHit hit;
        float minDistance = 1000;

        foreach (Vector3 v in edges)
        {
            if (Physics.Raycast(v, Vector3.down, out hit))
            {
                if (minDistance > hit.distance)
                {
                    minDistance = hit.distance;
                }
            }
        }

        _fallingDown = false;
        if (minDistance < fallDistance)
        {
            if (minDistance > 0.1f) // don't allow falling through the ground
            {
                transform.Translate(Vector3.down * 0.5f * minDistance);
                _fallingDown = true;
            }
        }
        else
        {
            transform.Translate(Vector3.down * fallDistance);
            _fallingDown = true;
        }
    }
}
