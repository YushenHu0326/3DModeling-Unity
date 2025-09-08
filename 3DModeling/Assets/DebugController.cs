using UnityEngine;
using System.Collections;

public class DebugController : MonoBehaviour
{
    public CanvasGrid grid;
    PointCloud pointCloud;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointCloud = new GameObject().AddComponent<PointCloud>();
        pointCloud.gameObject.transform.position = grid.gameObject.transform.position;
        pointCloud.val = 0.001f;

        StartCoroutine(AddPointCloud());
    }

    IEnumerator AddPointCloud()
    {
        yield return new WaitForSeconds(1f);
        if (grid)
        {
            for (int i = 0; i < 1000; i++)
            {
                pointCloud.AddPoint(new Vector3(Random.Range(0, grid.canvasSize), Random.Range(0, grid.canvasSize), Random.Range(0, grid.canvasSize)));
                grid.UpdatePointCloud(pointCloud);
                yield return new WaitForSeconds(0.02f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
