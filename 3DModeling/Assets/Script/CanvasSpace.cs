using UnityEngine;
using System.Collections;

public class CanvasSpace : MonoBehaviour
{
    public int xNum, yNum, zNum;

    public float gridSize;
    public int gridDivisions;

    public float surfaceValue;

    public bool useGPU;

    public Material mat;

    public ComputeShader marchingTetrahedraShader;

    CanvasGrid[] canvasGrids;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGrids = new CanvasGrid[xNum * yNum * zNum];
        for (int i = 0; i < xNum; i++)
        {
            for (int j = 0; j < yNum; j++)
            {
                for (int k = 0; k < zNum; k++)
                {
                    GameObject gridObject = new GameObject();
                    CanvasGrid grid = gridObject.AddComponent<CanvasGrid>();

                    grid.xIndex = i;
                    grid.yIndex = j;
                    grid.zIndex = k;

                    grid.canvasSize = gridSize;
                    grid.grid = gridDivisions;
                    grid.surfaceVal = surfaceValue;
                    grid.totalGridNum = xNum * yNum * zNum;
                    grid.useGPU = useGPU;
                    grid.marchingTetrahedraShader = marchingTetrahedraShader;

                    gridObject.transform.position = gameObject.transform.position + new Vector3(i * gridSize, j * gridSize, k * gridSize);

                    canvasGrids[k + j * zNum + i * yNum * zNum] = grid;
                }
            }
        }

        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(0.5f);


    }

    public CanvasGrid GetCanvasGrid(int id)
    {
        return canvasGrids[id];
    }
}
