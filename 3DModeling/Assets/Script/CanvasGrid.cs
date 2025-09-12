using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasGrid : MonoBehaviour
{
    // the class that stores information for each vertex on a grid
    public class GridInfo
    {
        public int x;
        public int y;
        public int z;
        public Vector3 position;
        public float val;
    }

    // the cube that will be used to compute the actual mesh, consists of 8 vertices from the grid
    public struct Cube
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

    // the struct of cube but for GPU computing
    public struct Tetrahedra
    {
        public Vector3 x;
        public Vector3 y;
        public Vector3 z;
        public Vector3 w;

        public float xVal;
        public float yVal;
        public float zVal;
        public float wVal;

        public Vector3 t1a;
        public Vector3 t1b;
        public Vector3 t1c;
        public Vector3 t1n;

        public Vector3 t2a;
        public Vector3 t2b;
        public Vector3 t2c;
        public Vector3 t2n;
    }

    public class Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public Vector3 n;
    }

    // the size of the grid canvas
    public float canvasSize = 100f;

    // the number of division
    public int grid = 10;

    // the maximum value stored on each vertex
    public float maxVal = 1f;

    // grid/cell size computed on start
    float gridSize;

    public bool useGPU;

    public List<GridInfo> gridInfos;

    Cube[] cubes;
    Tetrahedra[] tetras;

    MeshFilter filter;
    MeshRenderer meshRenderer;
    Mesh mesh;

    public ComputeShader marchingTetrahedraShader;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitGrid();

        InitCube();

        filter = gameObject.GetComponent<MeshFilter>();
        if (filter == null) filter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();

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

                    gridInfos.Add(info);
                }
            }
        }

        foreach (GridInfo info in gridInfos)
        {
            if (info.x > 2 && info.x < 8)
                if (info.y > 2 && info.y < 8)
                    if (info.z > 2 && info.z < 8)
                        info.val = 1f;
        }
    }

    void InitCube()
    {
        cubes = new Cube[grid * grid * grid];
        tetras = new Tetrahedra[grid * grid * grid * 6];

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

                    cubes[z + grid * y + grid * grid * x] = cube;

                    Tetrahedra tetra1 = new Tetrahedra();
                    tetra1.x = cube.x1y1z1.position;
                    tetra1.y = cube.x2y1z2.position;
                    tetra1.z = cube.x2y1z1.position;
                    tetra1.w = cube.x2y2z2.position;
                    tetra1.xVal = cube.x1y1z1.val;
                    tetra1.yVal = cube.x2y1z2.val;
                    tetra1.zVal = cube.x2y1z1.val;
                    tetra1.wVal = cube.x2y2z2.val;
                    tetras[(z + grid * y + grid * grid * x) * 6] = tetra1;

                    Tetrahedra tetra2 = new Tetrahedra();
                    tetra2.x = cube.x1y1z1.position;
                    tetra2.y = cube.x2y1z1.position;
                    tetra2.z = cube.x2y2z1.position;
                    tetra2.w = cube.x2y2z2.position;
                    tetra2.xVal = cube.x1y1z1.val;
                    tetra2.yVal = cube.x2y1z1.val;
                    tetra2.zVal = cube.x2y2z1.val;
                    tetra2.wVal = cube.x2y2z2.val;
                    tetras[(z + grid * y + grid * grid * x) * 6 + 1] = tetra2;

                    Tetrahedra tetra3 = new Tetrahedra();
                    tetra3.x = cube.x1y1z1.position;
                    tetra3.y = cube.x2y2z1.position;
                    tetra3.z = cube.x1y2z1.position;
                    tetra3.w = cube.x2y2z2.position;
                    tetra3.xVal = cube.x1y1z1.val;
                    tetra3.yVal = cube.x2y2z1.val;
                    tetra3.zVal = cube.x1y2z1.val;
                    tetra3.wVal = cube.x2y2z2.val;
                    tetras[(z + grid * y + grid * grid * x) * 6 + 2] = tetra3;

                    Tetrahedra tetra4 = new Tetrahedra();
                    tetra4.x = cube.x1y1z1.position;
                    tetra4.y = cube.x1y2z1.position;
                    tetra4.z = cube.x1y2z2.position;
                    tetra4.w = cube.x2y2z2.position;
                    tetra4.xVal = cube.x1y1z1.val;
                    tetra4.yVal = cube.x1y2z1.val;
                    tetra4.zVal = cube.x1y2z2.val;
                    tetra4.wVal = cube.x2y2z2.val;
                    tetras[(z + grid * y + grid * grid * x) * 6 + 3] = tetra4;

                    Tetrahedra tetra5 = new Tetrahedra();
                    tetra5.x = cube.x1y1z1.position;
                    tetra5.y = cube.x1y2z2.position;
                    tetra5.z = cube.x1y1z2.position;
                    tetra5.w = cube.x2y2z2.position;
                    tetra5.xVal = cube.x1y1z1.val;
                    tetra5.yVal = cube.x1y2z2.val;
                    tetra5.zVal = cube.x1y1z2.val;
                    tetra5.wVal = cube.x2y2z2.val;
                    tetras[(z + grid * y + grid * grid * x) * 6 + 4] = tetra5;

                    Tetrahedra tetra6 = new Tetrahedra();
                    tetra6.x = cube.x1y1z1.position;
                    tetra6.y = cube.x1y1z2.position;
                    tetra6.z = cube.x2y1z2.position;
                    tetra6.w = cube.x2y2z2.position;
                    tetra6.xVal = cube.x1y1z1.val;
                    tetra6.yVal = cube.x1y1z2.val;
                    tetra6.zVal = cube.x2y1z2.val;
                    tetra6.wVal = cube.x2y2z2.val;
                    tetras[(z + grid * y + grid * grid * x) * 6 + 5] = tetra6;
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
    }

    public void UpdateMesh()
    {
        if (mesh == null) return;

        if (cubes == null) return;

        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int verticesCount = 0;

        if (useGPU)
        {
            ComputeBuffer tetraBuffer = new ComputeBuffer(tetras.Length, sizeof(float) * 40);
            tetraBuffer.SetData(tetras);
            marchingTetrahedraShader.SetBuffer(0, "tetras", tetraBuffer);
            marchingTetrahedraShader.SetFloat("maxVal", maxVal);
            marchingTetrahedraShader.SetFloat("gridSize", gridSize);
            marchingTetrahedraShader.Dispatch(0, tetras.Length / 10, 1, 1);

            tetraBuffer.GetData(tetras);
            tetraBuffer.Dispose();

            for (int i = 0; i < cubes.Length; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (tetras[i * 6 + j].t1n.magnitude > 0f)
                    {
                        vertices.Add(tetras[i * 6 + j].t1a);
                        vertices.Add(tetras[i * 6 + j].t1b);
                        vertices.Add(tetras[i * 6 + j].t1c);

                        Vector3 n = Vector3.Cross(tetras[i * 6 + j].t1b - tetras[i * 6 + j].t1a, tetras[i * 6 + j].t1c - tetras[i * 6 + j].t1a);
                        if (Vector3.Dot(n, tetras[i * 6 + j].t1n) > 0f)
                        {
                            triangles.Add(verticesCount);
                            triangles.Add(verticesCount + 1);
                            triangles.Add(verticesCount + 2);
                        }
                        else
                        {
                            triangles.Add(verticesCount);
                            triangles.Add(verticesCount + 2);
                            triangles.Add(verticesCount + 1);
                        }

                        verticesCount += 3;
                    }

                    if (tetras[i * 6 + j].t2n.magnitude > 0f)
                    {
                        vertices.Add(tetras[i * 6 + j].t2a);
                        vertices.Add(tetras[i * 6 + j].t2b);
                        vertices.Add(tetras[i * 6 + j].t2c);

                        Vector3 n = Vector3.Cross(tetras[i * 6 + j].t2b - tetras[i * 6 + j].t2a, tetras[i * 6 + j].t2c - tetras[i * 6 + j].t2a);
                        if (Vector3.Dot(n, tetras[i * 6 + j].t2n) > 0f)
                        {
                            triangles.Add(verticesCount);
                            triangles.Add(verticesCount + 1);
                            triangles.Add(verticesCount + 2);
                        }
                        else
                        {
                            triangles.Add(verticesCount);
                            triangles.Add(verticesCount + 2);
                            triangles.Add(verticesCount + 1);
                        }

                        verticesCount += 3;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < cubes.Length; i++)
            {
                Cube cube = cubes[i];
                List<Triangle> t1 = MarchingTetrahedra(cube.x1y1z1, cube.x2y1z2, cube.x2y1z1, cube.x2y2z2);
                List<Triangle> t2 = MarchingTetrahedra(cube.x1y1z1, cube.x2y1z1, cube.x2y2z1, cube.x2y2z2);
                List<Triangle> t3 = MarchingTetrahedra(cube.x1y1z1, cube.x2y2z1, cube.x1y2z1, cube.x2y2z2);
                List<Triangle> t4 = MarchingTetrahedra(cube.x1y1z1, cube.x1y2z1, cube.x1y2z2, cube.x2y2z2);
                List<Triangle> t5 = MarchingTetrahedra(cube.x1y1z1, cube.x1y2z2, cube.x1y1z2, cube.x2y2z2);
                List<Triangle> t6 = MarchingTetrahedra(cube.x1y1z1, cube.x1y1z2, cube.x2y1z2, cube.x2y2z2);

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

                        Vector3 n = Vector3.Cross(t.b - t.a, t.c - t.a);
                        if (Vector3.Dot(n, t.n) > 0f)
                        {
                            triangles.Add(verticesCount);
                            triangles.Add(verticesCount + 1);
                            triangles.Add(verticesCount + 2);
                        }
                        else
                        {
                            triangles.Add(verticesCount);
                            triangles.Add(verticesCount + 2);
                            triangles.Add(verticesCount + 1);
                        }

                        verticesCount += 3;
                    }
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
            triangle.a = y.position + Vector3.Normalize(x.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.b = z.position + Vector3.Normalize(x.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.c = w.position + Vector3.Normalize(x.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.n = x.position - y.position;
            triangles.Add(triangle);
        }

        if (x.val != 0f && y.val + z.val + w.val == 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = x.position + Vector3.Normalize(y.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.b = x.position + Vector3.Normalize(z.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.c = x.position + Vector3.Normalize(w.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.n = y.position - x.position;
            triangles.Add(triangle);
        }

        if (y.val == 0f && x.val * z.val * w.val != 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = x.position + Vector3.Normalize(y.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.b = z.position + Vector3.Normalize(y.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.c = w.position + Vector3.Normalize(y.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.n = y.position - x.position;
            triangles.Add(triangle);
        }

        if (y.val != 0f && x.val + z.val + w.val == 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = y.position + Vector3.Normalize(x.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.b = y.position + Vector3.Normalize(z.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.c = y.position + Vector3.Normalize(w.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.n = x.position - y.position;
            triangles.Add(triangle);
        }

        if (z.val == 0f && y.val * x.val * w.val != 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = x.position + Vector3.Normalize(z.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.b = y.position + Vector3.Normalize(z.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.c = w.position + Vector3.Normalize(z.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.n = z.position - y.position;
            triangles.Add(triangle);
        }

        if (z.val != 0f && y.val + x.val + w.val == 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = z.position + Vector3.Normalize(x.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.b = z.position + Vector3.Normalize(y.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.c = z.position + Vector3.Normalize(w.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.n = y.position - z.position;
            triangles.Add(triangle);
        }

        if (w.val == 0f && y.val * z.val * x.val != 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = x.position + Vector3.Normalize(w.position - x.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.b = y.position + Vector3.Normalize(w.position - y.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.c = z.position + Vector3.Normalize(w.position - z.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.n = w.position - y.position;
            triangles.Add(triangle);
        }

        if (w.val != 0f && y.val + z.val + x.val == 0f)
        {
            Triangle triangle = new Triangle();
            triangle.a = w.position + Vector3.Normalize(x.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.b = w.position + Vector3.Normalize(y.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.c = w.position + Vector3.Normalize(z.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle.n = y.position - w.position;
            triangles.Add(triangle);
        }

        if (x.val + y.val == 0 && z.val * w.val != 0f)
        {
            Triangle triangle1 = new Triangle();
            Triangle triangle2 = new Triangle();

            triangle1.a = z.position + Vector3.Normalize(x.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.b = w.position + Vector3.Normalize(x.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.c = w.position + Vector3.Normalize(y.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.n = x.position - z.position;

            triangle2.a = z.position + Vector3.Normalize(y.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.b = w.position + Vector3.Normalize(y.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.c = z.position + Vector3.Normalize(x.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.n = x.position - z.position;

            triangles.Add(triangle1);
            triangles.Add(triangle2);
        }

        if (x.val * y.val != 0 && z.val + w.val == 0f)
        {
            Triangle triangle1 = new Triangle();
            Triangle triangle2 = new Triangle();

            triangle1.a = x.position + Vector3.Normalize(z.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.b = x.position + Vector3.Normalize(w.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.c = y.position + Vector3.Normalize(w.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.n = z.position - x.position;

            triangle2.a = y.position + Vector3.Normalize(z.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.b = y.position + Vector3.Normalize(w.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.c = x.position + Vector3.Normalize(z.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.n = z.position - x.position;

            triangles.Add(triangle1);
            triangles.Add(triangle2);
        }

        if (x.val + z.val == 0 && y.val * w.val != 0f)
        {
            Triangle triangle1 = new Triangle();
            Triangle triangle2 = new Triangle();

            triangle1.a = y.position + Vector3.Normalize(x.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.b = w.position + Vector3.Normalize(x.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.c = w.position + Vector3.Normalize(z.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.n = x.position - y.position;

            triangle2.a = y.position + Vector3.Normalize(z.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.b = w.position + Vector3.Normalize(z.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.c = y.position + Vector3.Normalize(x.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.n = x.position - y.position;

            triangles.Add(triangle1);
            triangles.Add(triangle2);
        }

        if (x.val * z.val != 0 && y.val + w.val == 0f)
        {
            Triangle triangle1 = new Triangle();
            Triangle triangle2 = new Triangle();

            triangle1.a = x.position + Vector3.Normalize(y.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.b = x.position + Vector3.Normalize(w.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.c = z.position + Vector3.Normalize(w.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.n = y.position - x.position;

            triangle2.a = z.position + Vector3.Normalize(y.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.b = z.position + Vector3.Normalize(w.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.c = x.position + Vector3.Normalize(y.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.n = y.position - x.position;

            triangles.Add(triangle1);
            triangles.Add(triangle2);
        }

        if (x.val + w.val == 0 && z.val * y.val != 0f)
        {
            Triangle triangle1 = new Triangle();
            Triangle triangle2 = new Triangle();

            triangle1.a = z.position + Vector3.Normalize(x.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.b = y.position + Vector3.Normalize(x.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.c = y.position + Vector3.Normalize(w.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.n = x.position - z.position;

            triangle2.a = z.position + Vector3.Normalize(w.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.b = y.position + Vector3.Normalize(w.position - y.position) * Mathf.Clamp(y.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.c = z.position + Vector3.Normalize(x.position - z.position) * Mathf.Clamp(z.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.n = x.position - z.position;

            triangles.Add(triangle1);
            triangles.Add(triangle2);
        }

        if (x.val * w.val != 0 && z.val + y.val == 0f)
        {
            Triangle triangle1 = new Triangle();
            Triangle triangle2 = new Triangle();

            triangle1.a = x.position + Vector3.Normalize(z.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.b = x.position + Vector3.Normalize(y.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.c = w.position + Vector3.Normalize(z.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle1.n = z.position - x.position;

            triangle2.a = w.position + Vector3.Normalize(z.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.b = w.position + Vector3.Normalize(y.position - w.position) * Mathf.Clamp(w.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.c = x.position + Vector3.Normalize(y.position - x.position) * Mathf.Clamp(x.val / maxVal * gridSize / 2f, 0f, gridSize / 2f);
            triangle2.n = z.position - x.position;

            triangles.Add(triangle1);
            triangles.Add(triangle2);
        }

        return triangles;
    }

    public void SetGrid(int x, int y, int z, float val)
    {
        GridInfo gridInfo = FindGrid(x, y, z);
        if (gridInfo != null) gridInfo.val = val;
    }

    // Helper function to locate a grid vertex
    public GridInfo FindGrid(int x, int y, int z)
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
