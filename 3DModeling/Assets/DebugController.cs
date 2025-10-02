using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Mathematics;

public class DebugController : MonoBehaviour
{
    public CanvasSpace canvas;

    public TextAsset data;

    [System.Serializable]
    public class SDF
    {
        public float[] sdf;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AddPointCloud());
    }

    IEnumerator AddPointCloud()
    {
        yield return new WaitForSeconds(1f);

        SDF sdfData = JsonUtility.FromJson<SDF>(data.ToString());

        CanvasGrid grid = canvas.GetCanvasGrid(0);

        float[] oGrid = new float[65 * 65 * 65];

        for (int x = 0; x < 65; x++)
        {
            for (int y = 0; y < 65; y++)
            {
                for (int z = 0; z < 65; z++)
                {
                    oGrid[z + 65 * y + 65 * 65 * x] = sdfData.sdf[z + 65 * y + 65 * 65 * x];
                }
            }
        }

        float[] newGrid = Blur(6, oGrid, 65);

        for (int x = 0; x < 65; x++)
        {
            for (int y = 0; y < 65; y++)
            {
                for (int z = 0; z < 65; z++)
                {
                    grid.SetGrid(x, y, z, newGrid[z + 65 * y + 65 * 65 * x]);
                }
            }
        }

        grid.UpdateMesh();
    }

    public float[] Blur(int iteration, float[] grid, int size)
    {
        float[] gridCopy = new float[size * size * size];

        for (int i = 0; i < iteration; i++)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        gridCopy[z + size * y + size * size * x] = grid[z + size * y + size * size * x];
                    }
                }
            }

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        int xMin = Mathf.Max(0, x - 3);
                        int xMax = Mathf.Min(size, x + 3);

                        int yMin = Mathf.Max(0, y - 3);
                        int yMax = Mathf.Min(size, y + 3);

                        int zMin = Mathf.Max(0, z - 3);
                        int zMax = Mathf.Min(size, z + 3);

                        float avg = 0f;
                        for (int xx = xMin; xx < xMax; xx++)
                        {
                            for (int yy = yMin; yy < yMax; yy++)
                            {
                                for (int zz = zMin; zz < zMax; zz++)
                                {
                                    avg += gridCopy[zz + 65 * yy + 65 * 65 * xx];
                                }
                            }
                        }

                        avg /= ((xMax - xMin) * (yMax - yMin) * (zMax - zMin));

                        grid[z + size * y + size * size * x] = avg;
                    }
                }
            }
        }

        return grid;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
