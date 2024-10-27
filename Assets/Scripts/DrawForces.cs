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

    private void AddLineRenderer(int i)
    {
        //LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        GameObject child = CreateChild(i);
        LineRenderer lr = child.AddComponent<LineRenderer>();

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = 0.05F;
        lr.endWidth = 0.2F;
        lr.useWorldSpace = false;
        lr.startColor = colors[i % colors.Count];
        lr.endColor = colors[i % colors.Count];
        lr.enabled = false; // Disable initially

        lineRenderers.Add(lr);
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
                    lineRenderers[i].enabled = true;
                    lineRenderers[i].positionCount = 2;
                    lineRenderers[i].SetPosition(0, Vector3.zero);
                    lineRenderers[i].SetPosition(1, -forceVectorList[i]);
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
}
