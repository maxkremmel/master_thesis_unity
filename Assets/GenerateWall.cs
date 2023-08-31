using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWall : MonoBehaviour
{
    Mesh wall;
    MeshRenderer meshRenderer;
    MeshFilter mf;

    public float PointSize = 1.0f;
    public Color color = Color.white;

    Vector3[] vertices = new Vector3[100*10];
    Color[] colors = new Color[100*10];

    // Start is called before the first frame update
    void Start()
    {
        wall = new Mesh();

        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        mf = gameObject.AddComponent<MeshFilter>();
        meshRenderer.material = new Material(Shader.Find("Point Cloud/Disk"));
        meshRenderer.material.SetFloat("_PointSize", PointSize);
        CreateWall();
        UpdateMesh();
    }

    void CreateWall()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                vertices[i*100 + j] = new Vector3(0, i * 0.1f, j * 0.1f); ;
                colors[i*100 +j] = new Color(color.r, color.g, color.b);
            }
        }
    }

    void UpdateMesh()
    {
        wall.Clear();
        wall.vertices = vertices;
        wall.colors = colors;

        int[] indices = new int[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            indices[i] = i;
        }

        wall.SetIndices(indices, MeshTopology.Points, 0);

        mf.mesh = wall;
    }
}
