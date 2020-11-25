using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Rendering;

public class Node : MonoBehaviour
{
    public float Desire = float.MaxValue;

    public Vector3 Position;
    public List<NodeEdge> AdjacentEdges = new List<NodeEdge>();

    public bool SetStart;
    public bool SetEnd;

    private bool IsStart;
    private bool IsEnd;

    private void Start()
    {
        Position = GetComponent<Transform>().position;
        SetStart = false;
        SetEnd = false;
    }

    private void Update()
    {
        if (SetStart)
        {
            SetAsStartPosition();
            IsStart = true;
            SetStart = false;
        }
        if (SetEnd)
        {
            SetAsEndPosition();
            IsEnd = true;
            SetEnd = false;
        }
    }

    public void ShowDesire()
    {
        if(Desire > 1.0f)
        {
            GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            return;
        }
        float RedAmount = Desire;
        float BlueAmount = 1 - RedAmount;

        GetComponent<Renderer>().material.SetColor("_Color", Color.red * RedAmount + Color.green * BlueAmount);
        Debug.Log("Desire is " + Desire);
    }

    public void SetAsStartPosition()
    {
        this.transform.localScale *= 2;
        NodeController.Instance.SetStartPos(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.z));
        NodeController.Instance.StartNode = this;
    }

    public void SetAsEndPosition()
    {
        this.transform.localScale *= 2;
        NodeController.Instance.SetEndPos(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.z));
        NodeController.Instance.EndNode = this;
    }

    public void Execute()
    {
        NodeController Cont = NodeController.Instance;
        Node TargetNode = GetClosestLowScoringNode(this);
        Debug.DrawLine(this.Position, TargetNode.Position,Color.red,10.0f,false);
        if (TargetNode != Cont.EndNode)
        {
            TargetNode.Execute();
        }
    }
    static Node GetClosestLowScoringNode(Node CallingNode)
    {
        Node theNode = null;
        float distance = 1000.0f;
        float desire = 1000.0f;
        foreach (Node node in NodeController.Instance.NodeList)
        {
            if (node == CallingNode) continue;
            if ((node.Desire < desire) && (Vector3.Distance(node.Position, CallingNode.Position) < distance))
            {
                theNode = node;
                desire = theNode.Desire;
                distance = Vector3.Distance(CallingNode.Position, node.Position);
            }
        }
        return theNode;
    }


}

