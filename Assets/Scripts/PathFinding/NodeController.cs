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
    private ComputeBuffer VertexBuffer;
    private ComputeBuffer IndexBuffer;

    private Vector3 StartPos;
    private Vector3 EndPos;

    private Vector3[] BufferNodePositionData;
    private Vector3Int[] BufferNodeIndexData;
    private float[] BufferNodeScoreData;
    private int Kernal;

    public bool DispatchCompute;
    public bool bIterateDispatch;

    public Node StartNode;
    public Node EndNode;

    NavMeshTriangulation ActiveNavmesh;

    // Start is called before the first frame update
    void Start()
    {
        ActiveNavmesh = NavMesh.CalculateTriangulation();

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
        if(bIterateDispatch)
        { 
            bIterateDispatch = false;
            IterateDispatch();
        }
    }

    public void SetSpawn()
    {
        SpawnGrid = true;
    }

    //todo Create a buffer to read and write to for the path finding.


    void DoSpawn()
    {
        BufferNodePositionData = ActiveNavmesh.vertices; //contains position data for each vertex.
        Debug.Log("Number of elements " + ActiveNavmesh.indices.Length + ". Number of faces" + ActiveNavmesh.indices.Length / 3);
        BufferNodeIndexData = new Vector3Int[ActiveNavmesh.indices.Length / 3]; //Each face is made up of 3 indices, each pointing to point to a vertex. SO this vec3 stores them.
        int i;
        for (i = 0; i < BufferNodeIndexData.Length; i ++)
        {
            BufferNodeIndexData[i].x = ActiveNavmesh.indices[i * 3]; //point 1 of triangle for face
            BufferNodeIndexData[i].y = ActiveNavmesh.indices[i * 3 + 1]; //point 2 of triangle for face
            BufferNodeIndexData[i].z = ActiveNavmesh.indices[i * 3 + 2]; //point 3 of triangle for face
        }

        BufferNodeScoreData = new float[BufferNodePositionData.Length];

        for (i = 0; i < BufferNodePositionData.Length; i++)
        {
            BufferNodeScoreData[i] = float.MaxValue;
            GameObject Obj = Instantiate(node, transform, false);
            Obj.transform.position = BufferNodePositionData[i];
            GridNode ObjNode = Obj.GetComponent<GridNode>();
        }
        Debug.Log("Number of nodes" + i);


        //set buffers
        VertexBuffer = new ComputeBuffer(BufferNodePositionData.Length, 3 * sizeof(float)); //xyz floats
        VertexBuffer.SetData(BufferNodePositionData);

        IndexBuffer = new ComputeBuffer(BufferNodeIndexData.Length, 3 * sizeof(int)); //1 2 3 indices to make up a face.
        IndexBuffer.SetData(BufferNodeIndexData);

        ScoreBuffer = new ComputeBuffer(BufferNodeScoreData.Length, sizeof(float)); // 1 score for each index. 3 Per face
        ScoreBuffer.SetData(BufferNodeScoreData);

        Kernal = PathFindingComputeShader.FindKernel("CSPath");
        PathFindingComputeShader.SetBuffer(Kernal, "NodeVertex", VertexBuffer);
        PathFindingComputeShader.SetBuffer(Kernal, "NodeIndices", IndexBuffer);
        PathFindingComputeShader.SetBuffer(Kernal, "NodeScore", ScoreBuffer);

        //set uniforms
        float[] Pos = new float[3] { 0, 0, 0};
        StartPos = Vector3.zero;

        PathFindingComputeShader.SetFloats("StartPos", Pos);
        Pos[0] = 5;
        Pos[1] = 10;
        Pos[2] = 15;
        EndPos.x = 5;
        EndPos.y = 10;
        EndPos.z = 15;
        PathFindingComputeShader.SetFloats("EndPos", Pos);
        Vector4 Direction = new Vector4(Pos[0], Pos[1], Pos[2]);
        Direction = Direction.normalized;
        Pos[0] = Direction.x;
        Pos[1] = Direction.y;
        Pos[2] = Direction.z;
        PathFindingComputeShader.SetFloats("Direction", Pos[0], Pos[1], Pos[2]);
        
        PathFindingComputeShader.SetFloats("MaxDistance", Vector3.Distance(StartPos, EndPos));
    }

    public void DispatchComputeShader()
    {
        PathFindingComputeShader.Dispatch(Kernal, BufferNodeIndexData.Length, 1, 1); 

        NodeList.AddRange(GetComponentsInChildren<Node>());
        ScoreBuffer.GetData(BufferNodeScoreData);
        for (int i = 0; i < NodeList.Count; i++)
        {
            NodeList[i].Desire = BufferNodeScoreData[i];
            NodeList[i].ShowDesire();
            //print("Desire of " + i + " is" + NodeList[i].Desire);
        }
    }

    public void IterateDispatch()
    {
        PathFindingComputeShader.Dispatch(Kernal, BufferNodeIndexData.Length, 1, 1);

        NodeList.AddRange(GetComponentsInChildren<Node>());
        ScoreBuffer.GetData(BufferNodeScoreData);
        for (int i = 0; i < NodeList.Count; i++)
        {
            NodeList[i].ShowDesire();
        }
    }

    public void SetStartPos(Vector3 VecIn)
    {
        StartPos = VecIn;
        float[] Positions = new float[3] { VecIn.x, VecIn.y,VecIn.z };
        PathFindingComputeShader.SetFloats("StartPos", Positions);
    }

    public void SetEndPos(Vector3 VecIn)
    {
        EndPos = VecIn;
        float[] Positions = new float[3] { VecIn.x, VecIn.y,VecIn.z };
        PathFindingComputeShader.SetFloats("EndPos", Positions);
        Vector3 TheDirection = EndPos - StartPos;
        TheDirection = TheDirection.normalized;
        Positions[0] = TheDirection.x;
        Positions[1] = TheDirection.y;
        Positions[2] = TheDirection.z;
        PathFindingComputeShader.SetFloats("Direction", Positions);

        PathFindingComputeShader.SetFloats("MaxDistance", Vector3.Distance(StartPos, EndPos));

        Debug.Log("Distance is " + Vector3.Distance(StartPos, EndPos));
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
//    private ComputeBuffer VertexBuffer;

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

//        VertexBuffer = new ComputeBuffer(BufferPosData.Length, 2 * sizeof(float));
//        VertexBuffer.SetData(BufferPosData);

//        ScoreBuffer = new ComputeBuffer(ScoreData.Length, 1 * sizeof(float));
//        ScoreBuffer.SetData(ScoreData);

//        Kernal = PathFindingComputeShader.FindKernel("CSPath");
//        PathFindingComputeShader.SetBuffer(Kernal, "NodePos", VertexBuffer);
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
