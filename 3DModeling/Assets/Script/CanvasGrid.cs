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

    public class Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
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
    MeshRenderer renderer;
    Mesh mesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitGrid();

        InitCube();

        filter = gameObject.GetComponent<MeshFilter>();
        if (filter == null) filter = gameObject.AddComponent<MeshFilter>();

        renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer == null) renderer = gameObject.AddComponent<MeshRenderer>();

        Vector3[] newVertices = new Vector3[0];
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

    void Update()
    {
        
    }

    void InitGrid()
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

        gridInfos[13].val = 0.2f;
    }

    void InitCube()
    {
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
                cube.x1y1z1.val += Mathf.Max(gridSize - Vector3.Distance(cube.x1y1z1.position, relPos), 0f) * pointCloud.val;
                cube.x2y1z1.val += Mathf.Max(gridSize - Vector3.Distance(cube.x2y1z1.position, relPos), 0f) * pointCloud.val;
                cube.x1y2z1.val += Mathf.Max(gridSize - Vector3.Distance(cube.x1y2z1.position, relPos), 0f) * pointCloud.val;
                cube.x2y2z1.val += Mathf.Max(gridSize - Vector3.Distance(cube.x2y2z1.position, relPos), 0f) * pointCloud.val;
                cube.x1y1z2.val += Mathf.Max(gridSize - Vector3.Distance(cube.x1y1z2.position, relPos), 0f) * pointCloud.val;
                cube.x2y1z2.val += Mathf.Max(gridSize - Vector3.Distance(cube.x2y1z2.position, relPos), 0f) * pointCloud.val;
                cube.x1y2z2.val += Mathf.Max(gridSize - Vector3.Distance(cube.x1y2z2.position, relPos), 0f) * pointCloud.val;
                cube.x2y2z2.val += Mathf.Max(gridSize - Vector3.Distance(cube.x2y2z2.position, relPos), 0f) * pointCloud.val;
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

    public void UpdateMesh()
    {
        if (mesh == null) return;

        if (cubes == null) return;

        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int verticesCount = 0;

        for (int i = 0; i < cubes.Count; i++)
        {
            Cube cube = cubes[i];
            List<Triangle> t1 = MarchingTetrahedra(cube.x1y1z1, cube.x2y1z2, cube.x2y1z1, cube.x2y2z2);
            List<Triangle> t2 = MarchingTetrahedra(cube.x1y1z1, cube.x2y1z1, cube.x2y2z1, cube.x2y2z2);
            List<Triangle> t3 = MarchingTetrahedra(cube.x1y1z1, cube.x2y2z1, cube.x1y2z1, cube.x2y2z2);
            List<Triangle> t4 = MarchingTetrahedra(cube.x1y1z1, cube.x1y2z1, cube.x1y2z2, cube.x2y2z2);
            List<Triangle> t5 = MarchingTetrahedra(cube.x1y1z1, cube.x1y1z2, cube.x1y2z2, cube.x2y2z2);
            List<Triangle> t6 = MarchingTetrahedra(cube.x1y1z1, cube.x2y1z2, cube.x1y1z2, cube.x2y2z2);

            t1.AddRange(t2);
            t1.AddRange(t3);
            t1.AddRange(t4);
            t1.AddRange(t5);
            t1.AddRange(t6);

            if (t1.Count > 0)
            {
                foreach (Triangle t in t1)
                {
                    vertices.Add(t.a);
                    vertices.Add(t.b);
                    vertices.Add(t.c);
                    triangles.Add(verticesCount);
                    triangles.Add(verticesCount + 1);
                    triangles.Add(verticesCount + 2);
                    verticesCount += 3;
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
    }

    List<Triangle> MarchingTetrahedra(GridInfo x, GridInfo y, GridInfo z, GridInfo w)
    {
        List<Triangle> triangles = new List<Triangle>();

        if (x.val == 0f && y.val * z.val * w.val != 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = y.position + Vector3.Normalize(x.position - y.position) * Mathf.Clamp(y.val, 0f, gridSize);
            triangle.b = z.position + Vector3.Normalize(x.position - z.position) * Mathf.Clamp(z.val, 0f, gridSize);
            triangle.c = w.position + Vector3.Normalize(x.position - w.position) * Mathf.Clamp(w.val, 0f, gridSize);
            triangles.Add(triangle);
        }

        if (x.val != 0f && y.val + z.val + w.val == 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = x.position + Vector3.Normalize(y.position - x.position) * Mathf.Clamp(x.val, 0f, gridSize);
            triangle.b = x.position + Vector3.Normalize(z.position - x.position) * Mathf.Clamp(x.val, 0f, gridSize);
            triangle.c = x.position + Vector3.Normalize(w.position - x.position) * Mathf.Clamp(x.val, 0f, gridSize);
            triangles.Add(triangle);
        }

        if (y.val == 0f && x.val * z.val * w.val != 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = x.position + Vector3.Normalize(y.position - x.position) * Mathf.Clamp(x.val, 0f, gridSize);
            triangle.b = z.position + Vector3.Normalize(y.position - z.position) * Mathf.Clamp(y.val, 0f, gridSize);
            triangle.c = w.position + Vector3.Normalize(y.position - w.position) * Mathf.Clamp(w.val, 0f, gridSize);
            triangles.Add(triangle);
        }

        if (y.val != 0f && x.val + z.val + w.val == 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = y.position + Vector3.Normalize(x.position - y.position) * Mathf.Clamp(y.val, 0f, gridSize);
            triangle.b = y.position + Vector3.Normalize(z.position - y.position) * Mathf.Clamp(y.val, 0f, gridSize);
            triangle.c = y.position + Vector3.Normalize(w.position - y.position) * Mathf.Clamp(y.val, 0f, gridSize);
            triangles.Add(triangle);
        }

        if (z.val == 0f && y.val * x.val * w.val != 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = x.position + Vector3.Normalize(z.position - x.position) * Mathf.Clamp(x.val, 0f, gridSize);
            triangle.b = y.position + Vector3.Normalize(z.position - y.position) * Mathf.Clamp(y.val, 0f, gridSize);
            triangle.c = w.position + Vector3.Normalize(z.position - w.position) * Mathf.Clamp(w.val, 0f, gridSize);
            triangles.Add(triangle);
        }

        if (z.val != 0f && y.val + x.val + w.val == 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = z.position + Vector3.Normalize(x.position - z.position) * Mathf.Clamp(z.val, 0f, gridSize);
            triangle.b = z.position + Vector3.Normalize(y.position - z.position) * Mathf.Clamp(z.val, 0f, gridSize);
            triangle.c = z.position + Vector3.Normalize(w.position - z.position) * Mathf.Clamp(z.val, 0f, gridSize);
            triangles.Add(triangle);
        }

        if (w.val == 0f && y.val * z.val * x.val != 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = x.position + Vector3.Normalize(w.position - x.position) * Mathf.Clamp(x.val, 0f, gridSize);
            triangle.b = y.position + Vector3.Normalize(w.position - y.position) * Mathf.Clamp(y.val, 0f, gridSize);
            triangle.c = z.position + Vector3.Normalize(w.position - z.position) * Mathf.Clamp(z.val, 0f, gridSize);
            triangles.Add(triangle);
        }

        if (w.val != 0f && y.val + z.val + x.val == 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = w.position + Vector3.Normalize(x.position - w.position) * Mathf.Clamp(w.val, 0f, gridSize);
            triangle.b = w.position + Vector3.Normalize(y.position - w.position) * Mathf.Clamp(w.val, 0f, gridSize);
            triangle.c = w.position + Vector3.Normalize(z.position - w.position) * Mathf.Clamp(w.val, 0f, gridSize);
            triangles.Add(triangle);
        }

        //if (x.val + y.val == 0 && z.)

        return triangles;
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
