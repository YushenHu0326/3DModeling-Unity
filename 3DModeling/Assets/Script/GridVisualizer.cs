using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    MeshRenderer renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void UpdateVisualizer(float val)
    {
        gameObject.transform.localScale = new Vector3(val, val, val);
    }
}
