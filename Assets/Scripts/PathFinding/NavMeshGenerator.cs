using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour
{
    public static NavMeshGenerator Instance;

    NavMeshTriangulation ActiveNavmesh;
    public Mesh VisualisedMesh = new Mesh();

    void start()
    {
        ActiveNavmesh = NavMesh.CalculateTriangulation();

        VisualisedMesh.vertices = ActiveNavmesh.vertices;
        VisualisedMesh.triangles = ActiveNavmesh.indices;

        for(int i = 0; i < 6; i ++)
        {
            Debug.Log("position " + i + " is " + VisualisedMesh.triangles[i]);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
