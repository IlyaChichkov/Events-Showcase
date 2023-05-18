using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UES;

public class PolyMapGenerator : MonoBehaviour
{
    [SerializeField] private Material triangleMaterial;
    [SerializeField] private Transform mapParent;
    public int gridSize = 10;
    public float frequency = 1f;
    public float amplitude = 1f;

    [ConsoleCommand("generate-map", "map", "")]
    void GeneratePerlinNoise(float frequency, float amplitude)
    {
        Vector3[] points = new Vector3[(gridSize) * (gridSize)];

        float xOffset = Random.Range(0f, 100f);
        float yOffset = Random.Range(0f, 100f);

        float stepSize = 1f / gridSize;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                float xCoord = x * stepSize * frequency + xOffset;
                float yCoord = y * stepSize * frequency + yOffset;

                float perlinValue = Mathf.PerlinNoise(xCoord, yCoord) * amplitude;

                points[x * (gridSize) + y] = new Vector3(x, perlinValue, y);
            }
        }

        GenerateMeshes(points);
    }

    [ConsoleCommand("destroy-map", "map", "")]
    public void DeleteMap()
    {
        while (mapParent.childCount > 0)
        {
            DestroyImmediate(mapParent.GetChild(0).gameObject);
            // DestroyImmediate(mapParent.GetChild(0));
        }
    }


    [ConsoleCommand("generate-mesh", "map", "")]
    public Mesh GenerateTriangleMesh(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { point1, point2, point3 };
        mesh.triangles = new int[] { 0, 1, 2 };
        mesh.RecalculateNormals();

        // Create a new game object with a mesh renderer and mesh filter
        GameObject meshObject = new GameObject("Generated Mesh");
        meshObject.transform.SetParent(mapParent);
        meshObject.transform.position = Vector3.zero;

        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshRenderer.material = triangleMaterial;

        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        return mesh;
    }

    public void GenerateMeshes(Vector3[] points)
    {
        DeleteMap();
        for (int y = 0; y < gridSize * gridSize - gridSize; y++)
        {
            GenerateTriangleMesh(points[y], points[y + gridSize], points[y + gridSize - 1]);
            GenerateTriangleMesh(points[y], points[y + 1], points[y + gridSize]);
        }
    }
}