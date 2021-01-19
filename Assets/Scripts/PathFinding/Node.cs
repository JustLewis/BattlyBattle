using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float Desire = float.MaxValue;

    public Vector3 Position;
<<<<<<< HEAD
    public List<NodeEdge> AdjacentEdges = new List<NodeEdge>();

    public bool SetStart;
    public bool SetEnd;

    private bool IsStart = false;
    private bool IsEnd = false;
=======
>>>>>>> parent of 4302454... Lots of changes... Just not been backing up because idiot

    private void Start()
    {
        Position = GetComponent<Transform>().position;
    }

    public void ShowDesire()
    {
<<<<<<< HEAD
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
=======
        GetComponent<Renderer>().material.SetColor("_Color", Color.white * (Desire / float.MaxValue));
>>>>>>> parent of 4302454... Lots of changes... Just not been backing up because idiot
    }
}