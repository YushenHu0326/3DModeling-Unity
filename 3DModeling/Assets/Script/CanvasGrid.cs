using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasGrid : MonoBehaviour
{
    public class GridInfo
    {
        public int x;
        public int y;
        public int z;
        public Vector3 position;
        public float val;

        public GridVisualizer visualizer;
    }

    public class Cube
    {
        public GridInfo x1y1z1;
        public GridInfo x2y1z1;
        public GridInfo x1y2z1;
        public GridInfo x2y2z1;
        public GridInfo x1y1z2;
        public GridInfo x2y1z2;
        public GridInfo x1y2z2;
        public GridInfo x2y2z2;
    }

    public float canvasSize = 100f;

    public int grid = 10;

    float gridSize;

    public List<GridInfo> gridInfos;

    public bool useVisualizer = true;

    public GameObject visualizerPrefab;

    public List<GridVisualizer> visualizers;

    List<Cube> cubes;

    MeshFilter filter;
    Mesh mesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridSize = canvasSize / (float)grid;
        gridInfos = new List<GridInfo>();

        for (int x = 0; x <= grid; x++)
        {
            for (int y = 0; y <= grid; y++)
            {
                for (int z = 0; z <= grid; z++)
                {
                    GridInfo info = new GridInfo();
                    info.x = x;
                    info.y = y;
                    info.z = z;
                    info.position = new Vector3(gridSize * (float)x, gridSize * (float)y, gridSize * (float)z);
                    info.val = 0f;

                    if (useVisualizer)
                    {
                        GridVisualizer visualizer = Instantiate(visualizerPrefab).GetComponent<GridVisualizer>();
                        info.visualizer = visualizer;
                        visualizer.gameObject.transform.position = info.position;
                        visualizer.UpdateVisualizer(info.val);
                    }

                    gridInfos.Add(info);
                }
            }
        }

        cubes = new List<Cube>();

        for (int x = 0; x < grid; x++)
        {
            for (int y = 0; y < grid; y++)
            {
                for (int z = 0; z < grid; z++)
                {
                    Cube cube = new Cube();
                    cube.x1y1z1 = FindGrid(x, y, z);
                    cube.x2y1z1 = FindGrid(x + 1, y, z);
                    cube.x1y2z1 = FindGrid(x, y + 1, z);
                    cube.x2y2z1 = FindGrid(x + 1, y + 1, z);
                    cube.x1y1z2 = FindGrid(x, y, z + 1);
                    cube.x2y1z2 = FindGrid(x + 1, y, z + 1);
                    cube.x1y2z2 = FindGrid(x, y + 1, z + 1);
                    cube.x2y2z2 = FindGrid(x + 1, y + 1, z + 1);

                    cubes.Add(cube);
                }
            }
        }

        filter = gameObject.GetComponent<MeshFilter>();
        if (filter == null) filter = gameObject.AddComponent<MeshFilter>();

        Vector3[] newVertices = new Vector3[cubes.Count * 12];
        Vector2[] newUVs = new Vector2[0];
        int[] newTriangles = new int[0];

        mesh = new Mesh
        {
            vertices = newVertices,
            uv = newUVs,
            triangles = newTriangles
        };

        filter.mesh = mesh;

        UpdateMesh();
    }

    public void UpdatePointCloud(PointCloud pointCloud)
    {
        foreach (Vector3 point in pointCloud.points)
        {
            Vector3 relPos = point - gameObject.transform.position;
            if ((relPos.x < canvasSize && relPos.y < canvasSize) && relPos.z < canvasSize)
            {
                int x = (int)Mathf.Floor(relPos.x / canvasSize * (float)grid);
                int y = (int)Mathf.Floor(relPos.y / canvasSize * (float)grid);
                int z = (int)Mathf.Floor(relPos.z / canvasSize * (float)grid);

                Cube cube = cubes[z + y * grid + x * grid * grid];
                cube.x1y1z1.val += (gridSize - Vector3.Distance(cube.x1y1z1.position, relPos)) * pointCloud.val;
                cube.x2y1z1.val += (gridSize - Vector3.Distance(cube.x2y1z1.position, relPos)) * pointCloud.val;
                cube.x1y2z1.val += (gridSize - Vector3.Distance(cube.x1y2z1.position, relPos)) * pointCloud.val;
                cube.x2y2z1.val += (gridSize - Vector3.Distance(cube.x2y2z1.position, relPos)) * pointCloud.val;
                cube.x1y1z2.val += (gridSize - Vector3.Distance(cube.x1y1z2.position, relPos)) * pointCloud.val;
                cube.x2y1z2.val += (gridSize - Vector3.Distance(cube.x2y1z2.position, relPos)) * pointCloud.val;
                cube.x1y2z2.val += (gridSize - Vector3.Distance(cube.x1y2z2.position, relPos)) * pointCloud.val;
                cube.x2y2z2.val += (gridSize - Vector3.Distance(cube.x2y2z2.position, relPos)) * pointCloud.val;
            }
        }

        if (useVisualizer)
        {
            foreach (GridInfo info in gridInfos)
            {
                info.visualizer.UpdateVisualizer(info.val);
            }
        }
    }

    void UpdateMesh()
    {
        if (mesh == null) return;

        if (cubes == null) return;

        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < cubes.Count; i++)
        {
            Cube cube = cubes[i];
            
        }
    }

    // Helper function to locate a grid vertex
    GridInfo FindGrid(int x, int y, int z)
    {
        foreach (GridInfo gridInfo in gridInfos)
        {
            if ((gridInfo.x == x && gridInfo.y == y) && gridInfo.z == z)
            {
                return gridInfo;
            }
        }

        return null;
    }
}
