using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public static NodeController Instance;
    public GameObject node;
    public int GridSize;
    public bool SpawnGrid;
    private List<Node> NodeList = new List<Node>();
   

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        if (SpawnGrid)
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    GameObject Obj = Instantiate(node, transform, false);
                    Obj.transform.position = new Vector3(i * 3, 0.5f, j * 3);
                    GridNode ObjNode = Obj.GetComponent<GridNode>();
                    ObjNode.NodePos = new Vector2Int(i, j);
                }
            }
        }
        NodeList.AddRange(GetComponentsInChildren<Node>());
        NodeList[5].Desire = float.MaxValue / 2;
        NodeList[5].ShowDesire();

    }

    //todo Create a buffer to read and write to for the path finding.
}
