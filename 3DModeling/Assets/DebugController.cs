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
        for (int x = 0; x < grid.grid; x++)
            for (int y = 0; y < grid.grid; y++)
                for (int z = 0; z < grid.grid; z++)
                    grid.SetGrid(x, y, z, Mathf.Max(0, Perlin.Noise((float)x / (float)grid.grid * 5f, (float)y / (float)grid.grid * 5f, (float)z / (float)grid.grid * 5f)));
        
        grid.UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
