using UnityEngine;
using System.Collections;

/// <summary>
/// Lightning effect. Can be used on empty game objects to create lightning stroke.
/// </summary>
public class Lightning : MonoBehaviour
{
    /// <summary>
    /// Where do we start the lightning?
    /// </summary>
    /// <remarks>Iff this is null we will use fromPosition as a static start point.</remarks>
    public Transform fromTransform;

    /// <summary>
    /// Iff fromTransform is null this is the static position. Otherwise this is a translation from fromTransform.
    /// </summary>
    public Vector3 fromPosition = Vector3.zero;

    /// <summary>
    /// See fromTransform.
    /// </summary>
    public Transform toTransform;

    /// <summary>
    /// See fromPosition.
    /// </summary>
    public Vector3 toPosition = Vector3.zero;

    /// <summary>
    /// How many points should be inserted between beginning and end?
    /// </summary>
    public int intermediatePoints;

    /// <summary>
    /// The material to use.
    /// </summary>
    public Material material;

    /// <summary>
    /// The line renderer of the corresponding game object.
    /// </summary>
    private LineRenderer lineRenderer;

    /// <summary>
    /// All points which define the line renderer.
    /// </summary>
    private Vector3[] points;

    /// <summary>
    /// How fast does the lightning move?
    /// </summary>
    public float speed = 10f;

    /// <summary>
    /// How much do the bending points move randomly?
    /// </summary>
    public float randomRange = 1f;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    private void Start()
    {
        if (intermediatePoints < 0)
        {
            intermediatePoints = 0;
        }

        // add line renderer iff it does not exist already
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            if (material != null)
            {
                lineRenderer.material = material;
            }
        }

        lineRenderer.SetVertexCount(2 + intermediatePoints);

        points = new Vector3[intermediatePoints + 2];

        Vector3 start = fromPosition;
        if (fromTransform != null)
        {
            start += fromTransform.position;
        }
        points[0] = start;
        lineRenderer.SetPosition(0, start);

        Vector3 end = toPosition;
        if (toTransform != null)
        {
            end += toTransform.position;
        }
        points[1 + intermediatePoints] = end;
        lineRenderer.SetPosition(1 + intermediatePoints, end);

        for (int i = 1; i <= intermediatePoints; i++)
        {
            points[i] = start + ((end - start) * i / (intermediatePoints + 1)) + (Random.insideUnitSphere * randomRange);
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (fromTransform != null)
        {
            ////points[0] = Vector3.Lerp(points[0], fromTransform.position + fromPosition, Time.deltaTime * speed * 10);
            points[0] = fromTransform.position + fromPosition;
            lineRenderer.SetPosition(0, points[0]);
        }
        if (toTransform != null)
        {
            ////points[1 + intermediatePoints] = Vector3.Lerp(points[1 + intermediatePoints], toTransform.position + toPosition, Time.deltaTime * speed * 10);
            points[1 + intermediatePoints] = toTransform.position + toPosition;
            lineRenderer.SetPosition(1 + intermediatePoints, points[1 + intermediatePoints]);
        }
        for (int i = 1; i <= intermediatePoints; i++)
        {
            points[i] = Vector3.Lerp(points[i], points[0] + ((points[1 + intermediatePoints] - points[0]) * i / (intermediatePoints + 1)) + (Random.insideUnitSphere * randomRange), Time.deltaTime * speed);
            lineRenderer.SetPosition(i, points[i]);
        }

    }

    /// <summary>
    /// Called when this component is disabled.
    /// </summary>
    private void OnDisable()
    {
        if (lineRenderer)
        {
            lineRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Called when this component is enabled.
    /// </summary>
    private void OnEnable()
    {
        if (lineRenderer)
        {
            lineRenderer.enabled = true;
        }
    }
}
