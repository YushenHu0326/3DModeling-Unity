using UnityEngine;
using System.Collections;

public class DebugController : MonoBehaviour
{
    public CanvasGrid grid;
    public PointCloud pointCloud;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AddPointCloud());
    }

    IEnumerator AddPointCloud()
    {
        yield return new WaitForSeconds(1f);
        if (grid)
        {
            for (int i = 0; i < 5000; i++)
            {
                Vector3 pos = new Vector3(Random.Range(0, grid.canvasSize), Random.Range(0, grid.canvasSize), Random.Range(0, grid.canvasSize));
                if (Vector3.Distance(pos, new Vector3(grid.canvasSize, grid.canvasSize, grid.canvasSize)) < grid.canvasSize / 2f)
                    pointCloud.AddPoint(pos);
                grid.UpdatePointCloud(pointCloud);
                //yield return new WaitForSeconds(0.01f);
            }
        }

        grid.UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
