using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingNavMesh : MonoBehaviour
{
    // Start is called before the first frame update

    List<MeshFilter> ChildRenderers = new List<MeshFilter>();
    void Start()
    {
        //ChildRenderers.AddRange(GetComponentsInChildren<MeshFilter>());

        //int NumberOfVerts = 0;

        //foreach (MeshFilter Child in ChildRenderers)
        //{
        //    //Child.GetComponent<NavMeshGenerator>().RecreateMesh();
            
        //}

        //Debug.Log("Number of verts is " + NumberOfVerts);

        //foreach (Vector3 Vertex in ChildRenderers[0].mesh.vertices)
        //{
        //    Debug.Log("Vector is " + Vertex.ToString());
        //}
    }


    // Update is called once per frame
    void Update()
    {

    }

    List<int> Nodes = new List<int>();
}