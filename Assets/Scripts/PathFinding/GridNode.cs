using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : Node
{
    public Vector2Int NodePos;
    public int GridSize;

    private float Height;

    private void Start()
    {
        //GridSize = NodeController.Instance.GridSize;
        Height = GetComponent<Transform>().position.y;


    }

}
