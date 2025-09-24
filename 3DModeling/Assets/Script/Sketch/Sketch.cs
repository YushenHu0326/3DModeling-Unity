using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sketch : MonoBehaviour
{
    List<Vector3> points;
    LineRenderer line;

    public float radius = 0.02f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        points = new List<Vector3>();
        line = GetComponent<LineRenderer>();
        if (line == null) line = gameObject.AddComponent<LineRenderer>();

        line.startColor = Color.black;
        line.endColor = Color.black;

        line.startWidth = 0.02f;
        line.endWidth = 0.02f;

        line.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Draw(float x, float y, float z)
    {
        Vector3 point = new Vector3(x, y, z);
        points.Add(point);

        line.positionCount += 1;
        line.SetPositions(points.ToArray());
    }

    public void SetPoints(List<Vector3> newPoints)
    {
        points = newPoints;

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }
}
