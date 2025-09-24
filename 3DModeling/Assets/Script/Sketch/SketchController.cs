using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SketchController : MonoBehaviour
{
    public GameObject sketchPrefab;

    List<Vector3Int> sketchPoints;

    List<Sketch> sketches;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sketches = new List<Sketch>();

        StartCoroutine(Test());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(1f);
        //GetSketchPointsFromPointCloud();
        Sketch sketch = Instantiate(sketchPrefab).GetComponent<Sketch>();
        sketches.Add(sketch);

        yield return new WaitForSeconds(0.1f);
        sketch.Draw(0.2f, 0.3f, 0.5f);
        yield return new WaitForSeconds(0.1f);
        sketch.Draw(0.3f, 0.3f, 0.5f);
        yield return new WaitForSeconds(0.1f);
        sketch.Draw(0.5f, 0.4f, 0.5f);
        yield return new WaitForSeconds(0.1f);
        sketch.Draw(0.5f, 0.5f, 0.5f);
        yield return new WaitForSeconds(0.1f);
        sketch.Draw(0.6f, 0.6f, 0.6f);
        yield return new WaitForSeconds(0.1f);
        sketch.Draw(0.7f, 0.7f, 0.8f);
        yield return new WaitForSeconds(0.1f);
        sketch.Draw(0.3f, 0.4f, 0.6f);
    }

    /*public void GetSketchPointsFromPointCloud()
    {
        if (canvas)
        {
            // z face
            // front
            List<int> zFaceFront = new List<int>();

            for (int x = 0; x < canvas.xNum; x++)
            {
                for (int y = 0; y < canvas.yNum; y++)
                {
                    int maxX = x == canvas.xNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;
                    int maxY = y == canvas.yNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;

                    for (int gridX = 0; gridX < maxX; gridX++)
                    {
                        for (int gridY = 0; gridY < maxY; gridY++)
                        {
                            int maxGridZ = 0;
                            int maxZVal = 0;

                            float maxVal = canvas.surfaceValue;

                            for (int z = 0; z < canvas.zNum; z++)
                            {
                                int maxZ = z == canvas.zNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;

                                for (int gridZ = 0; gridZ < maxZ; gridZ++)
                                {
                                    if (canvas.GetGrid(x, y, z, gridX, gridY, gridZ) > maxVal)
                                    {
                                        maxVal = canvas.GetGrid(x, y, z, gridX, gridY, gridZ);
                                        maxGridZ = gridZ + z * canvas.gridDivisions;
                                        maxZVal = z;
                                    }
                                }
                            }

                            zFaceFront.Add(maxGridZ);
                        }
                    }
                }
            }

            // back
            List<int> zFaceBack = new List<int>();

            for (int x = 0; x < canvas.xNum; x++)
            {
                for (int y = 0; y < canvas.yNum; y++)
                {
                    int maxX = x == canvas.xNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;
                    int maxY = y == canvas.yNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;

                    for (int gridX = 0; gridX < maxX; gridX++)
                    {
                        for (int gridY = 0; gridY < maxY; gridY++)
                        {
                            int maxGridZ = 0;
                            int maxZVal = 0;

                            float maxVal = canvas.surfaceValue;

                            for (int z = 0; z < canvas.zNum; z++)
                            {
                                int maxZ = z == canvas.zNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;

                                for (int gridZ = 0; gridZ < maxZ; gridZ++)
                                {
                                    if (canvas.GetGrid(x, y, z, gridX, gridY, gridZ) > maxVal)
                                    {
                                        maxVal = canvas.GetGrid(x, y, z, gridX, gridY, gridZ);
                                        maxGridZ = gridZ + z * canvas.gridDivisions;
                                        maxZVal = z;
                                        break;
                                    }
                                }
                            }

                            zFaceBack.Add(maxGridZ);
                        }
                    }
                }
            }

            // y face
            // front
            List<int> yFaceFront = new List<int>();

            for (int x = 0; x < canvas.xNum; x++)
            {
                for (int z = 0; z < canvas.zNum; z++)
                {
                    int maxX = x == canvas.xNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;
                    int maxZ = z == canvas.zNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;

                    for (int gridX = 0; gridX < maxX; gridX++)
                    {
                        for (int gridZ = 0; gridZ < maxZ; gridZ++)
                        {
                            int maxGridY = 0;
                            int maxYVal = 0;

                            float maxVal = canvas.surfaceValue;

                            for (int y = 0; y < canvas.yNum; y++)
                            {
                                int maxY = y == canvas.yNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;

                                for (int gridY = 0; gridY < maxY; gridY++)
                                {
                                    if (canvas.GetGrid(x, y, z, gridX, gridY, gridZ) > maxVal)
                                    {
                                        maxVal = canvas.GetGrid(x, y, z, gridX, gridY, gridZ);
                                        maxGridY = gridY + y * canvas.gridDivisions;
                                        maxYVal = y;
                                    }
                                }
                            }

                            yFaceFront.Add(maxGridY);
                        }
                    }
                }
            }

            // back
            List<int> yFaceBack = new List<int>();

            for (int x = 0; x < canvas.xNum; x++)
            {
                for (int z = 0; z < canvas.zNum; z++)
                {
                    int maxX = x == canvas.xNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;
                    int maxZ = z == canvas.zNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;

                    for (int gridX = 0; gridX < maxX; gridX++)
                    {
                        for (int gridZ = 0; gridZ < maxZ; gridZ++)
                        {
                            int maxGridY = 0;
                            int maxYVal = 0;

                            float maxVal = canvas.surfaceValue;

                            for (int y = 0; y < canvas.yNum; y++)
                            {
                                int maxY = y == canvas.yNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;

                                for (int gridY = 0; gridY < maxY; gridY++)
                                {
                                    if (canvas.GetGrid(x, y, z, gridX, gridY, gridZ) > maxVal)
                                    {
                                        maxVal = canvas.GetGrid(x, y, z, gridX, gridY, gridZ);
                                        maxGridY = gridY + y * canvas.gridDivisions;
                                        maxYVal = y;
                                        break;
                                    }
                                }
                            }

                            yFaceBack.Add(maxGridY);
                        }
                    }
                }
            }


            // x face
            // front
            List<int> xFaceFront = new List<int>();

            for (int y = 0; y < canvas.yNum; y++)
            {
                for (int z = 0; z < canvas.zNum; z++)
                {
                    int maxY = y == canvas.yNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;
                    int maxZ = z == canvas.zNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;

                    for (int gridY = 0; gridY < maxY; gridY++)
                    {
                        for (int gridZ = 0; gridZ < maxZ; gridZ++)
                        {
                            int maxGridX = 0;
                            int maxXVal = 0;

                            float maxVal = canvas.surfaceValue;

                            for (int x = 0; x < canvas.xNum; x++)
                            {
                                int maxX = x == canvas.xNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;

                                for (int gridX = 0; gridX < maxX; gridX++)
                                {
                                    if (canvas.GetGrid(x, y, z, gridX, gridY, gridZ) > maxVal)
                                    {
                                        maxVal = canvas.GetGrid(x, y, z, gridX, gridY, gridZ);
                                        maxGridX = gridX + x * canvas.gridDivisions;
                                        maxXVal = x;
                                    }
                                }
                            }

                            xFaceFront.Add(maxGridX);
                        }
                    }
                }
            }

            // back
            List<int> xFaceBack = new List<int>();

            for (int y = 0; y < canvas.yNum; y++)
            {
                for (int z = 0; z < canvas.zNum; z++)
                {
                    int maxY = y == canvas.yNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;
                    int maxZ = z == canvas.zNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;

                    for (int gridY = 0; gridY < maxY; gridY++)
                    {
                        for (int gridZ = 0; gridZ < maxZ; gridZ++)
                        {
                            int maxGridX = 0;
                            int maxXVal = 0;

                            float maxVal = canvas.surfaceValue;

                            for (int x = 0; x < canvas.xNum; x++)
                            {
                                int maxX = x == canvas.xNum - 1 ? canvas.gridDivisions + 1 : canvas.gridDivisions;

                                for (int gridX = 0; gridX < maxX; gridX++)
                                {
                                    if (canvas.GetGrid(x, y, z, gridX, gridY, gridZ) > maxVal)
                                    {
                                        maxVal = canvas.GetGrid(x, y, z, gridX, gridY, gridZ);
                                        maxGridX = gridX + x * canvas.gridDivisions;
                                        maxXVal = x;
                                        break;
                                    }
                                }
                            }

                            xFaceBack.Add(maxGridX);
                        }
                    }
                }
            }

            int[] zEdgeFront = new int[(canvas.gridDivisions * canvas.xNum + 1) * (canvas.gridDivisions * canvas.yNum + 1)];

            for (int x = 1; x < canvas.gridDivisions * canvas.xNum; x++)
            {
                for (int y = 1; y < canvas.gridDivisions * canvas.yNum; y++)
                {
                    int z = zFaceFront[y + x * canvas.gridDivisions * canvas.yNum];

                    if (z != 0)
                    {
                        if (zFaceFront[y - 1 + x * canvas.gridDivisions * canvas.yNum] == 0)
                            zEdgeFront[y + x * canvas.gridDivisions * canvas.yNum] = z;
                        if (zFaceFront[y + 1 + x * canvas.gridDivisions * canvas.yNum] == 0)
                            zEdgeFront[y + x * canvas.gridDivisions * canvas.yNum] = z;
                        if (zFaceFront[y + (x - 1) * canvas.gridDivisions * canvas.yNum] == 0)
                            zEdgeFront[y + x * canvas.gridDivisions * canvas.yNum] = z;
                        if (zFaceFront[y + (x + 1) * canvas.gridDivisions * canvas.yNum] == 0)
                            zEdgeFront[y + x * canvas.gridDivisions * canvas.yNum] = z;
                    }
                }
            }

            int[] zEdgeBack = new int[(canvas.gridDivisions * canvas.xNum + 1) * (canvas.gridDivisions * canvas.yNum + 1)];

            for (int x = 1; x < canvas.gridDivisions * canvas.xNum; x++)
            {
                for (int y = 1; y < canvas.gridDivisions * canvas.yNum; y++)
                {
                    int z = zFaceBack[y + x * canvas.gridDivisions * canvas.yNum];

                    if (z != 0)
                    {
                        if (zFaceFront[y - 1 + x * canvas.gridDivisions * canvas.yNum] == 0)
                            zEdgeFront[y + x * canvas.gridDivisions * canvas.yNum] = z;
                        if (zFaceFront[y + 1 + x * canvas.gridDivisions * canvas.yNum] == 0)
                            zEdgeFront[y + x * canvas.gridDivisions * canvas.yNum] = z;
                        if (zFaceFront[y + (x - 1) * canvas.gridDivisions * canvas.yNum] == 0)
                            zEdgeFront[y + x * canvas.gridDivisions * canvas.yNum] = z;
                        if (zFaceFront[y + (x + 1) * canvas.gridDivisions * canvas.yNum] == 0)
                            zEdgeFront[y + x * canvas.gridDivisions * canvas.yNum] = z;
                    }
                }
            }

            int[] yEdgeFront = new int[(canvas.gridDivisions * canvas.xNum + 1) * (canvas.gridDivisions * canvas.zNum + 1)];

            for (int x = 1; x < canvas.gridDivisions * canvas.xNum; x++)
            {
                for (int z = 1; z < canvas.gridDivisions * canvas.zNum; z++)
                {
                    int y = yFaceFront[z + x * canvas.gridDivisions * canvas.zNum];
                    for (int xx = x - 1; xx <= x + 1; xx++)
                    {
                        for (int zz = z - 1; zz <= z + 1; zz++)
                        {
                            int yNeighbor = yFaceFront[zz + xx * canvas.gridDivisions * canvas.zNum];
                            if ((float)Mathf.Abs(y - yNeighbor) >= sketchThreshold * (float)canvas.gridDivisions * (float)canvas.yNum)
                            {
                                yEdgeFront[z + x * canvas.gridDivisions * canvas.zNum] = y;
                                break;
                            }
                        }
                    }
                }
            }

            int[] yEdgeBack = new int[(canvas.gridDivisions * canvas.xNum + 1) * (canvas.gridDivisions * canvas.zNum + 1)];

            for (int x = 1; x < canvas.gridDivisions * canvas.xNum; x++)
            {
                for (int z = 1; z < canvas.gridDivisions * canvas.zNum; z++)
                {
                    int y = yFaceBack[z + x * canvas.gridDivisions * canvas.zNum];
                    for (int xx = x - 1; xx <= x + 1; xx++)
                    {
                        for (int zz = z - 1; zz <= z + 1; zz++)
                        {
                            int yNeighbor = yFaceBack[zz + xx * canvas.gridDivisions * canvas.zNum];
                            if ((float)Mathf.Abs(y - yNeighbor) >= sketchThreshold * (float)canvas.gridDivisions * (float)canvas.yNum)
                            {
                                yEdgeBack[z + x * canvas.gridDivisions * canvas.zNum] = y;
                                break;
                            }
                        }
                    }
                }
            }

            int[] xEdgeFront = new int[(canvas.gridDivisions * canvas.yNum + 1) * (canvas.gridDivisions * canvas.zNum + 1)];

            for (int y = 1; y < canvas.gridDivisions * canvas.yNum; y++)
            {
                for (int z = 1; z < canvas.gridDivisions * canvas.zNum; z++)
                {
                    int x = xFaceFront[z + y * canvas.gridDivisions * canvas.zNum];
                    for (int yy = y - 1; yy <= y + 1; yy++)
                    {
                        for (int zz = z - 1; zz <= z + 1; zz++)
                        {
                            int xNeighbor = xFaceFront[zz + yy * canvas.gridDivisions * canvas.zNum];
                            if ((float)Mathf.Abs(x - xNeighbor) >= sketchThreshold * (float)canvas.gridDivisions * (float)canvas.xNum)
                            {
                                xEdgeFront[z + y * canvas.gridDivisions * canvas.zNum] = x;
                                break;
                            }
                        }
                    }
                }
            }

            int[] xEdgeBack = new int[(canvas.gridDivisions * canvas.yNum + 1) * (canvas.gridDivisions * canvas.zNum + 1)];

            for (int y = 1; y < canvas.gridDivisions * canvas.yNum; y++)
            {
                for (int z = 1; z < canvas.gridDivisions * canvas.zNum; z++)
                {
                    int x = xFaceBack[z + y * canvas.gridDivisions * canvas.zNum];
                    for (int yy = y - 1; yy <= y + 1; yy++)
                    {
                        for (int zz = z - 1; zz <= z + 1; zz++)
                        {
                            int xNeighbor = xFaceBack[zz + yy * canvas.gridDivisions * canvas.zNum];
                            if ((float)Mathf.Abs(x - xNeighbor) >= sketchThreshold * (float)canvas.gridDivisions * (float)canvas.xNum)
                            {
                                xEdgeBack[z + y * canvas.gridDivisions * canvas.zNum] = x;
                                break;
                            }
                        }
                    }
                }
            }

            sketchPoints = new List<Vector3Int>();

            for (int x = 1; x < canvas.gridDivisions * canvas.xNum; x++)
            {
                for (int y = 1; y < canvas.gridDivisions * canvas.yNum; y++)
                {
                    if (zEdgeFront[y + x * canvas.gridDivisions * canvas.yNum] > 0)
                    {
                        Vector3Int point = new Vector3Int(x, y, zEdgeFront[y + x * canvas.gridDivisions * canvas.yNum]);
                        if (!sketchPoints.Contains(point))
                            sketchPoints.Add(point);
                    }
                }
            }

            for (int x = 1; x < canvas.gridDivisions * canvas.xNum; x++)
            {
                for (int y = 1; y < canvas.gridDivisions * canvas.yNum; y++)
                {
                    if (zEdgeBack[y + x * canvas.gridDivisions * canvas.yNum] > 0)
                    {
                        Vector3Int point = new Vector3Int(x, y, zEdgeBack[y + x * canvas.gridDivisions * canvas.yNum]);
                        if (!sketchPoints.Contains(point))
                            sketchPoints.Add(point);
                    }
                }
            }

            foreach (Vector3Int point in sketchPoints)
            {
                Instantiate(debug).transform.position = new Vector3(point.x, point.y, point.z) * (canvas.gridSize / canvas.gridDivisions) + canvas.gameObject.transform.position;
            }
            
            //first Make sure you're using RGB24 as your texture format
            Texture2D texture = new Texture2D(canvas.xNum * canvas.gridDivisions + 1, canvas.yNum * canvas.gridDivisions + 1, TextureFormat.RGB24, false);
            for (int x = 0; x < canvas.xNum * canvas.gridDivisions + 1; x++)
            {
                for (int y = 0; y < canvas.yNum * canvas.gridDivisions + 1; y++)
                {
                    texture.SetPixel(x, y, new Color((float)zEdgeFront[y + x * (canvas.xNum * canvas.gridDivisions + 1)] / 50f, 0f, 0f));
                }
            }

            //then Save To Disk as PNG
            byte[] bytes = texture.EncodeToPNG();
            var dirPath = Application.dataPath + "/../SaveImages/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(dirPath + "Image" + ".png", bytes);

            //first Make sure you're using RGB24 as your texture format
            texture = new Texture2D(canvas.xNum * canvas.gridDivisions + 1, canvas.yNum * canvas.gridDivisions + 1, TextureFormat.RGB24, false);
            for (int x = 0; x < canvas.xNum * canvas.gridDivisions + 1; x++)
            {
                for (int y = 0; y < canvas.yNum * canvas.gridDivisions + 1; y++)
                {
                    texture.SetPixel(x, y, new Color((float)zFaceFront[y + x * (canvas.xNum * canvas.gridDivisions + 1)] / 50f, 0f, 0f));
                }
            }

            //then Save To Disk as PNG
            bytes = texture.EncodeToPNG();
            File.WriteAllBytes(dirPath + "Image2" + ".png", bytes);
        }
    }*/
}
