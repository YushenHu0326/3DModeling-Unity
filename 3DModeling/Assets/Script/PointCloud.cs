using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointCloud : MonoBehaviour
{
    public List<Vector3> points;
    public float val = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPoint(Vector3 newPoint)
    {
        if (points == null) points = new List<Vector3>();
        points.Add(newPoint);
    }

    public void RemovePoint(Vector3 pointToRemove)
    {
        if (points == null) return;
        points.Remove(pointToRemove);
    }
}
