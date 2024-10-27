using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(PhysicsEngine))]
public class DrawForces : MonoBehaviour
{
    public bool showTrails = true;

    private List<Vector3> forceVectorList = new List<Vector3>();
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private List<Color> colors;

    // Use this for initialization
    void Start()
    {
        forceVectorList = GetComponent<PhysicsEngine>().forceVectorList;

        // Initialize colors
        colors = new List<Color> { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan };

        // Create a LineRenderer for each color
        for (int i = 0; i < colors.Count; i++)
        {
            AddLineRenderer(i);
        }
    }

    private GameObject CreateChild(int i)
    {
        GameObject child = new GameObject("Force Vector " + i);
        child.transform.parent = this.transform;
        child.transform.localPosition = Vector3.zero;

        return child;
    }

    private void CreateCone(Transform parent, Color color)
    {
        GameObject cone = new GameObject("Cone");
        cone.SetActive(false);
        cone.transform.parent = parent;

        MeshFilter mf = cone.AddComponent<MeshFilter>();
        MeshRenderer mr = cone.AddComponent<MeshRenderer>();

        mf.mesh = CreateConeMesh(0.2f, 0.1f, 20);

        // Create unlit material that ignores lighting 
        var material = new Material(Shader.Find("Unlit/Color"));
        material.color = color;

        // If Unlit/Color shader is not available, create an emissive material as fallback
        if (material.shader == null)
        {
            material = new Material(Shader.Find("Standard"));
            material.color = color;
            // Make the material emissive to be visible in all lighting conditions
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 0.5f);
        }

        mr.material = material;

        // Make sure the material renders on top of other objects
        mr.material.renderQueue = 3000; // Higher number renders later/on top
    }

    private void AddLineRenderer(int i)
    {
        //LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        GameObject child = CreateChild(i);
        LineRenderer lr = child.AddComponent<LineRenderer>();

        Color lineColor = colors[i % colors.Count];

        CreateCone(lr.transform, lineColor);

        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        lineMaterial.color = lineColor;
        if (lineMaterial.shader == null)
        {
            lineMaterial = new Material(Shader.Find("Sprites/Default"));
            lineMaterial.color = lineColor;
        }

        lr.material = lineMaterial;
        lr.startWidth = 0.1F;
        lr.endWidth = 0.1F;
        lr.useWorldSpace = false;
        //lr.startColor = lineColor;
        //lr.endColor = lineColor;
        lr.enabled = false; // Disable initially

        // Make lines render on top
        lr.material.renderQueue = 3000;

        lineRenderers.Add(lr);
    }

    private Mesh CreateConeMesh(float height, float radius, int segments)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // Create cone pointing in the forward direction (Z-axis)
        vertices[0] = new Vector3(0, 0, height); // Tip of cone at positive Z

        // Base vertices
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }

        // Center of the base
        vertices[segments + 1] = Vector3.zero;

        // Triangles
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 1) % segments + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    // Update is called once per frame
    void Update()
    {
        if (showTrails)
        {
            int numberOfForces = forceVectorList.Count;

            if (lineRenderers.Count < numberOfForces)
            {
                // add more LineRenderers
                for (int i = lineRenderers.Count; i < numberOfForces; i++)
                {
                    AddLineRenderer(i);
                }
            }

            for (int i = 0; i < lineRenderers.Count; i++)
            {
                if (i < numberOfForces)
                {
                    Vector3 force = forceVectorList[i];
                    DrawForce(i, force);
                }
                else
                {
                    lineRenderers[i].enabled = false;
                }
            }
        }
        else
        {
            foreach (var lr in lineRenderers)
            {
                lr.enabled = false;
            }
        }
    }

    private void DrawForce(int i, Vector3 force)
    {
        LineRenderer lr = lineRenderers[i];
        lr.enabled = true;

        // Main line
        lr.positionCount = 2;
        lr.SetPosition(0, Vector3.zero);
        lr.SetPosition(1, force);

        // Arrowhead
        if (lr.gameObject.transform.childCount > 0)
        {
            GameObject cone = lr.gameObject.transform.GetChild(0).gameObject;
            cone.SetActive(true);

            // Position the cone at the end of the line
            cone.transform.localPosition = force;

            // Calculate the rotation to align the cone with the force direction
            if (force != Vector3.zero)
            {
                // Create rotation from the forward direction (Z-axis) to the force direction
                Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, force.normalized);
                cone.transform.localRotation = targetRotation;

                /*
                // Scale the cone based on force magnitude
                float scale = force.magnitude * 0.1f; // Adjust multiplier as needed
                cone.transform.localScale = new Vector3(scale, scale, scale);
                */
            }
            else
            {
                cone.SetActive(false);
            }
        }
    }
}
