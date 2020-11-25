using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor.UI;
using UnityEngine;
using UnityEngineInternal;
using UnityEngine.AI;


public class NodeController : MonoBehaviour
{
    public static NodeController Instance;
    public GameObject node;
    public bool SpawnGrid;
    public List<Node> NodeList = new List<Node>();

    public ComputeShader PathFindingComputeShader;
    private ComputeBuffer ScoreBuffer;
    private ComputeBuffer PositionBuffer;
    private ComputeBuffer IndexBuffer;

    private Vector3 StartPos;
    private Vector3 EndPos;

    private Vector3[] BufferNodePositionData;
    private int[] BufferNodeIndexData;
    private float[] ScoreData;
    private int Kernal;

    public bool DispatchCompute;

    public Node StartNode;
    public Node EndNode;

    // Start is called before the first frame update
    void Start()
    {
        DispatchCompute = false;
        Instance = this;
        if (SpawnGrid)
        {
            DoSpawn();
        }

    }

    private void Update()
    {
        if (DispatchCompute)
        {
            DispatchComputeShader();
            DispatchCompute = false;
            //StartNode.Execute();
        }
    }

    //todo Create a buffer to read and write to for the path finding.


    void DoSpawn()
    {
        BufferNodePositionData = NavMeshGenerator.Instance.VisualisedMesh.vertices;
        BufferNodeIndexData = NavMeshGenerator.Instance.VisualisedMesh.triangles;
        ScoreData = new float[NavMeshGenerator.Instance.VisualisedMesh.triangles.Length];

        for (int i = 0; i < NavMeshGenerator.Instance.VisualisedMesh.triangles.Length; i++)
        {
            ScoreData[i] = float.MaxValue;
            GameObject Obj = Instantiate(node, transform, false);
            Obj.transform.position = BufferNodePositionData[i];
            GridNode ObjNode = Obj.GetComponent<GridNode>();
        }

        PositionBuffer = new ComputeBuffer(BufferNodePositionData.Length, 3 * sizeof(float));
        PositionBuffer.SetData(BufferNodePositionData);

        IndexBuffer = new ComputeBuffer(BufferNodeIndexData.Length, sizeof(int));
        IndexBuffer.SetData(BufferNodeIndexData);

        ScoreBuffer = new ComputeBuffer(ScoreData.Length, sizeof(float));
        ScoreBuffer.SetData(ScoreData);

        Kernal = PathFindingComputeShader.FindKernel("CSPath");
        PathFindingComputeShader.SetBuffer(Kernal, "NodePos", PositionBuffer);
        PathFindingComputeShader.SetBuffer(Kernal, "NodeIndex", IndexBuffer);
        PathFindingComputeShader.SetBuffer(Kernal, "NodeScore", ScoreBuffer);

        float[] Pos = new float[2] { 0, 0 };
        StartPos = Vector2.zero;

        PathFindingComputeShader.SetFloats("StartPos", Pos);
        Pos[0] = 5;
        Pos[1] = 10;
        EndPos.x = 5;
        EndPos.y = 10;
        PathFindingComputeShader.SetFloats("EndPos", Pos);
        Vector2 Direction = new Vector2(Pos[0], Pos[1]);
        Direction = Direction.normalized;
        Pos[0] = Direction.x;
        Pos[1] = Direction.y;
        PathFindingComputeShader.SetFloats("Direction", Pos[0], Pos[1]);
    }

    public void DispatchComputeShader()
    {
        PathFindingComputeShader.Dispatch(Kernal, NavMeshGenerator.Instance.VisualisedMesh.triangles.Length, 1, 1);

        NodeList.AddRange(GetComponentsInChildren<Node>());
        ScoreBuffer.GetData(ScoreData);
        for (int i = 0; i < NodeList.Count; i++)
        {
            NodeList[i].Desire = ScoreData[i];
            NodeList[i].ShowDesire();
            //print("Desire of " + i + " is" + NodeList[i].Desire);
        }
    }

    public void SetStartPos(Vector2 VecIn)
    {
        StartPos = VecIn;
        float[] Positions = new float[2] { VecIn.x, VecIn.y };
        PathFindingComputeShader.SetFloats("StartPos", Positions);
    }

    public void SetEndPos(Vector2 VecIn)
    {
        EndPos = VecIn;
        float[] Positions = new float[2] { VecIn.x, VecIn.y };
        PathFindingComputeShader.SetFloats("EndPos", Positions);
        Vector2 TheDirection = EndPos - StartPos;
        TheDirection = TheDirection.normalized;
        Positions[0] = TheDirection.x;
        Positions[1] = TheDirection.y;
        PathFindingComputeShader.SetFloats("Direction", Positions);
    }
}


///Old version


//public class OldNodeController : MonoBehaviour
//{
//    public static NodeController Instance;
//    public GameObject node;
//    public int GridSize;
//    public bool SpawnGrid;
//    public int GridScale;
//    public List<Node> NodeList = new List<Node>();

//    public ComputeShader PathFindingComputeShader;
//    private ComputeBuffer ScoreBuffer;
//    private ComputeBuffer PositionBuffer;

//    private Vector2 StartPos;
//    private Vector2 EndPos;

//    private Vector2[] BufferPosData; 
//    private float[] ScoreData;
//    private int Kernal;

//    public bool DispatchCompute;

//    public Node StartNode;
//    public Node EndNode;



//    // Start is called before the first frame update
//    void Start()
//    {
//        DispatchCompute = false;
//        Instance = this;
//        if (SpawnGrid)
//        {
//            DoSpawn();   
//        }
        
//    }

//    private void Update()
//    {
//        if(DispatchCompute)
//        {
//            DispatchComputeShader();
//            DispatchCompute = false;
//            //StartNode.Execute();
//        }
//    }

//    //todo Create a buffer to read and write to for the path finding.


//    void DoSpawn()
//    {
//        BufferPosData = new Vector2[GridSize * GridSize];
//        ScoreData = new float[GridSize * GridSize];

//        int xPos = 0; 
//        int yPos = 0; 

//        for (int i = 0; i < GridSize; i++)
//        {
//            for (int j = 0; j < GridSize; j++)
//            {
//                xPos = i * 3;
//                xPos = Random.Range(-GridScale, GridScale);
//                yPos = j * 3;
//                yPos = Random.Range(-GridScale, GridScale);
//                BufferPosData[GridSize * i + j].x = xPos;
//                BufferPosData[GridSize * i + j].y = yPos;
//                ScoreData[GridSize * i + j] = float.MaxValue;
//                GameObject Obj = Instantiate(node, transform, false);
//                Obj.transform.position = new Vector3(xPos, 1.5f, yPos);
//                GridNode ObjNode = Obj.GetComponent<GridNode>();
//                //ObjNode.NodePos = new Vector2Int(i, j);
//            }
//        }

//        PositionBuffer = new ComputeBuffer(BufferPosData.Length, 2 * sizeof(float));
//        PositionBuffer.SetData(BufferPosData);

//        ScoreBuffer = new ComputeBuffer(ScoreData.Length, 1 * sizeof(float));
//        ScoreBuffer.SetData(ScoreData);

//        Kernal = PathFindingComputeShader.FindKernel("CSPath");
//        PathFindingComputeShader.SetBuffer(Kernal, "NodePos", PositionBuffer);
//        PathFindingComputeShader.SetBuffer(Kernal, "NodeScore", ScoreBuffer);

//        float[] Pos = new float[2]{ 0,0};
//        StartPos = Vector2.zero;

//        PathFindingComputeShader.SetFloats("StartPos", Pos);
//        Pos[0] = 5;
//        Pos[1] = 10;
//        EndPos.x = 5;
//        EndPos.y = 10;
//        PathFindingComputeShader.SetFloats("EndPos", Pos);
//        Vector2 Direction = new Vector2(Pos[0], Pos[1]);
//        Direction = Direction.normalized;
//        Pos[0] = Direction.x;
//        Pos[1] = Direction.y;
//        PathFindingComputeShader.SetFloats("Direction", Pos[0], Pos[1]);
//    }

//    public void DispatchComputeShader()
//    {
//        PathFindingComputeShader.Dispatch(Kernal, GridSize * GridSize, 1, 1);

//        NodeList.AddRange(GetComponentsInChildren<Node>());
//        ScoreBuffer.GetData(ScoreData);
//        for (int i = 0; i < NodeList.Count; i++)
//        {
//            NodeList[i].Desire = ScoreData[i];
//            NodeList[i].ShowDesire();
//            //print("Desire of " + i + " is" + NodeList[i].Desire);
//        }
//    }

//    public void SetStartPos(Vector2 VecIn)
//    {
//        StartPos = VecIn;
//        float[] Positions = new float[2] { VecIn.x, VecIn.y };
//        PathFindingComputeShader.SetFloats("StartPos", Positions);
//    }

//    public void SetEndPos(Vector2 VecIn)
//    {
//        EndPos = VecIn;
//        float[] Positions = new float[2] { VecIn.x, VecIn.y };
//        PathFindingComputeShader.SetFloats("EndPos", Positions);
//        Vector2 TheDirection = EndPos - StartPos;
//        TheDirection = TheDirection.normalized;
//        Positions[0] = TheDirection.x;
//        Positions[1] = TheDirection.y;
//        PathFindingComputeShader.SetFloats("Direction", Positions);
//    }
//}
