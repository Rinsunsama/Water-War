using UnityEngine;
using System.Collections;

//Grid manager class handles all the grid properties
public class GridManager : MonoBehaviour
{

    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static GridManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static GridManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first GridManager object in the scene.
                s_Instance = FindObjectOfType(typeof(GridManager)) as GridManager;
                if (s_Instance == null)
                    Debug.Log("Could not locate an GridManager object. \n You have to have exactly one GridManager in the scene.");
            }
            return s_Instance;
        }
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit()
    {
        s_Instance = null;
    }

    #region Fields
    public int numOfRows;
    public int numOfColumns;
    public float gridCellSize;
    public bool showGrid = true;
    public bool showObstacleBlocks = true;

    private Vector3 origin = new Vector3();//��һ����index = 0
    private GameObject[] obstacleList;//�ϰ����б�
    public GameObject[] play1_heroList;//���1Ӣ���б�
    public GameObject[] play2_heroList;//���2Ӣ���б�
    public Node[,] nodes { get; set; }//��Ԫ������
    #endregion

    //Origin of the grid manager
    public Vector3 Origin
    {
        get { return origin; }
    }

    //Initialise the grid manager
    void Awake()
    {

    }
    void Update()
    {

    }
    [RPC]
    public void findObstacleAndCalculateWaterPower()
    {
        obstacleList = GameObject.FindGameObjectsWithTag("Obstacle");
        play1_heroList = GameObject.FindGameObjectsWithTag("Player1_hero");
        play2_heroList = GameObject.FindGameObjectsWithTag("Player2_hero");
        CalculateObstacles();//���ϰ������ڵĵ�Ԫ����Ϊ���ϰ���
        CalculateHero();//��Ӣ�����ڵĵ�Ԫ����Ϊ���ϰ���
     //   scanWaterEyesAndCalculateWaterPower();
    }

    void scanWaterEyesAndCalculateWaterPower()
    {
        GameManager.waterPower = (GameManager.roundIndex > 6) ? 10 : 5 + GameManager.roundIndex - 1;        //����ʥˮ��
        if (GameManager.player_side == GameManager.Player_side.player1)
        {
            if (play1_heroList != null && play1_heroList.Length > 0)
            {
                foreach (GameObject data in play1_heroList)
                {
                    if ((data.transform.position.x == GameManager.waterEyes[0].x) && (data.transform.position.z == GameManager.waterEyes[0].z))
                    {
                        GameManager.waterPower++;
                        break;
                    }
                    if ((data.transform.position.x == GameManager.waterEyes[1].x) && (data.transform.position.z == GameManager.waterEyes[1].z))
                    {
                        GameManager.waterPower++;
                        break;
                    }
                    if ((data.transform.position.x == GameManager.waterEyes[2].x) && (data.transform.position.z == GameManager.waterEyes[2].z))
                    {
                        GameManager.waterPower += 2;
                        break;
                    }
                }
            }
        }
        if (GameManager.player_side == GameManager.Player_side.player2)
        {
            if (play2_heroList != null && play2_heroList.Length > 0)
            {
                foreach (GameObject data in play2_heroList)
                {
                    if ((data.transform.position.x == GameManager.waterEyes[0].x) && (data.transform.position.z == GameManager.waterEyes[0].z))
                    {
                        GameManager.waterPower++;
                        break;
                    }
                    if ((data.transform.position.x == GameManager.waterEyes[1].x) && (data.transform.position.z == GameManager.waterEyes[1].z))
                    {
                        GameManager.waterPower++;
                        break;
                    }
                    if ((data.transform.position.x == GameManager.waterEyes[2].x) && (data.transform.position.z == GameManager.waterEyes[2].z))
                    {
                        GameManager.waterPower += 2;
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// ���ݵ�Ԫ�񣬻�����ڵ�Ԫ�������е�λ��
    /// </summary>
    public Node CalculateAtNodes(Node node)
    {
        int nodeIndex = GetGridIndex(node.position);//�ڵ���
        int row = GetRow(nodeIndex);//��
        int col = GetColumn(nodeIndex);//��
        return nodes[row, col];
    }
    /// <summary>
    /// ���ݵ�Ԫ���ж����Ƿ����ϰ���λ��
    /// </summary>
    public bool IsObstacle(Node node)
    {
        int nodeIndex = GetGridIndex(node.position);//�ڵ���
        int row = GetRow(nodeIndex);//��
        int col = GetColumn(nodeIndex);//��
        if (nodes[row, col].bObstacle)
            return true;
        else
            return false;
    }
    void CalculateObstacles()
    {
        nodes = new Node[numOfRows, numOfColumns];//��ʼ����Ԫ������

        //�����е�Ԫ��(node)���뵥Ԫ������(nodes[,])
        int index = 0;
        for (int i = 0; i < numOfRows; i++)
        {
            for (int j = 0; j < numOfColumns; j++)
            {
                Vector3 cellPos = GetGridCellPosition(index);
                Node node = new Node(cellPos);
                nodes[i, j] = node;
                index++;
            }
        }

        //�����ϰ����б��е�λ�õĽڵ㣬���Ϊ���ϰ���
        if (obstacleList != null && obstacleList.Length > 0)
        {
            foreach (GameObject data in obstacleList)
            {
                int indexCell = GetGridIndex(data.transform.position);
                int col = GetColumn(indexCell);
                int row = GetRow(indexCell);

                //Also make the node as blocked status
                nodes[row, col].MarkAsObstacle();
            }
        }
    }
    [RPC]
    //������Ӣ�����ڵ��ϰ����־ȡ��
    public void ClearAllObstacle()
    {
        for (int i = 0; i < numOfRows; i++)
        {
            for (int j = 0; j < numOfColumns; j++)
            {
                if (nodes[i, j].bObstacle)
                    nodes[i, j].ClearAsObstacle();
            }
        }
    }
    //������Ӣ�����ڵĵ�Ԫ����Ϊ���ϰ���
    void CalculateHero()
    {
        //�����1Ӣ���б��е�λ�õĽڵ㣬���Ϊ���ϰ���
        if (play1_heroList != null && play1_heroList.Length > 0)
        {
            foreach (GameObject data in play1_heroList)
            {
                int indexCell = GetGridIndex(data.transform.position);
                int col = GetColumn(indexCell);
                int row = GetRow(indexCell);

                nodes[row, col].MarkAsObstacle();
            }
        }

        //�����1Ӣ���б��е�λ�õĽڵ㣬���Ϊ���ϰ���
        if (play2_heroList != null && play2_heroList.Length > 0)
        {
            foreach (GameObject data in play2_heroList)
            {
                int indexCell = GetGridIndex(data.transform.position);
                int col = GetColumn(indexCell);
                int row = GetRow(indexCell);
                nodes[row, col].MarkAsObstacle();
            }
        }
    }

    /// <summary>
    /// ���ݵ�Ԫ���ţ���ȡ��Ԫ��λ�õ�����
    /// ��������ʱû��
    /// </summary>
    public Vector3 GetGridCellCenter(int index)
    {
        Vector3 cellPosition = GetGridCellPosition(index);
        cellPosition.x += (gridCellSize / 2.0f);
        cellPosition.z += (gridCellSize / 2.0f);

        return cellPosition;
    }

    /// <summary>
    /// ���ݵ�Ԫ���ţ���ȡ��Ԫ��λ��
    /// </summary>
    public Vector3 GetGridCellPosition(int index)
    {
        int row = GetRow(index);
        int col = GetColumn(index);
        float xPosInGrid = col * gridCellSize;
        float zPosInGrid = row * gridCellSize;

        return Origin + new Vector3(xPosInGrid, 0.0f, zPosInGrid);
    }
    /// <summary>
    /// ���ݵ�Ԫ��λ�ã���ø�λ�õ�Ԫ����
    /// </summary>
    public int GetGridIndex(Vector3 pos)
    {
        if (!IsInBounds(pos))
        {
            return -1;
        }

        pos -= Origin;

        int col = (int)(pos.x / gridCellSize);
        int row = (int)(pos.z / gridCellSize);

        return (row * numOfColumns + col);
    }
    public int GetGridIndex(Node node)
    {
        Vector3 pos = node.position;
        if (!IsInBounds(pos))
        {
            return -1;
        }

        pos -= Origin;

        int col = (int)(pos.x / gridCellSize);
        int row = (int)(pos.z / gridCellSize);

        return (row * numOfColumns + col);
    }
    /// <summary>
    /// ���ݱ�ţ���õ�Ԫ�����row
    /// </summary>
    public int GetRow(int index)
    {
        int row = index / numOfColumns;
        return row;
    }

    /// <summary>
    /// ���ݱ�ţ���õ�Ԫ�����col
    /// </summary>
    public int GetColumn(int index)
    {
        int col = index % numOfColumns;
        return col;
    }

    /// <summary>
    /// �ж�λ���Ƿ��ڷ�Χ(��ͼ)��
    /// </summary>
    public bool IsInBounds(Vector3 pos)
    {
        float width = numOfColumns * gridCellSize;
        float height = numOfRows * gridCellSize;

        return (pos.x >= Origin.x && pos.x <= Origin.x + width && pos.x <= Origin.z + height && pos.z >= Origin.z);
    }

    /// <summary>
    /// ���ڸ�����node����ȡ�����ĸ�����ɴ���ھӽ��
    /// </summary>
    public void GetNeighbours(Node node, ArrayList neighbors)
    {
        Vector3 neighborPos = node.position;
        int neighborIndex = GetGridIndex(neighborPos);

        int row = GetRow(neighborIndex);
        int column = GetColumn(neighborIndex);

        //��
        int leftNodeRow = row - 1;
        int leftNodeColumn = column;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //��
        leftNodeRow = row + 1;
        leftNodeColumn = column;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //��
        leftNodeRow = row;
        leftNodeColumn = column + 1;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //��
        leftNodeRow = row;
        leftNodeColumn = column - 1;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
    }
    public void GetNeighboursIncludeObstacle(Node node, ArrayList neighbors)
    {
        Vector3 neighborPos = node.position;
        int neighborIndex = GetGridIndex(neighborPos);

        int row = GetRow(neighborIndex);
        int column = GetColumn(neighborIndex);

        //��
        int leftNodeRow = row - 1;
        int leftNodeColumn = column;
        AssignNeighbourIncludeObstacle(leftNodeRow, leftNodeColumn, neighbors);

        //��
        leftNodeRow = row + 1;
        leftNodeColumn = column;
        AssignNeighbourIncludeObstacle(leftNodeRow, leftNodeColumn, neighbors);

        //��
        leftNodeRow = row;
        leftNodeColumn = column + 1;
        AssignNeighbourIncludeObstacle(leftNodeRow, leftNodeColumn, neighbors);

        //��
        leftNodeRow = row;
        leftNodeColumn = column - 1;
        AssignNeighbourIncludeObstacle(leftNodeRow, leftNodeColumn, neighbors);
    }

    /// <summary>
    /// ���ڵ㣺����û�г�����Χ������û�б����Ϊ�ϰ���
    /// Ȼ�󽫽ڵ�����ھӽڵ��б�neighbors
    /// </summary>
    /// <param name='row'>
    /// Row.
    /// </param>
    /// <param name='column'>
    /// Column.
    /// </param>
    /// <param name='neighbors'>
    /// Neighbors.
    /// </param>
    void AssignNeighbour(int row, int column, ArrayList neighbors)
    {
        if (row != -1 && column != -1 && row < numOfRows && column < numOfColumns)
        {
            Node nodeToAdd = nodes[row, column];
            if (!nodeToAdd.bObstacle)
            {
                neighbors.Add(nodeToAdd);
            }
        }
    }
    void AssignNeighbourIncludeObstacle(int row, int column, ArrayList neighbors)
    {
        if (row != -1 && column != -1 && row < numOfRows && column < numOfColumns)
        {
            Node nodeToAdd = nodes[row, column];
            //if (!nodeToAdd.bObstacle)
            //{
            neighbors.Add(nodeToAdd);
            //}
        }
    }
    /// <summary>
    /// Show Debug Grids and obstacles inside the editor
    /// </summary>
    //void OnDrawGizmos()
    //{
    //    //Draw Grid
    //    if (showGrid)
    //    {
    //        DebugDrawGrid(transform.position, numOfRows, numOfColumns, gridCellSize, Color.blue);
    //    }

    //    //Grid Start Position
    //    Gizmos.DrawSphere(transform.position, 0.5f);

    //    //Draw Obstacle obstruction
    //    if (showObstacleBlocks)
    //    {
    //        Vector3 cellSize = new Vector3(gridCellSize, 1.0f, gridCellSize);

    //        if (obstacleList != null && obstacleList.Length > 0)
    //        {
    //            foreach (GameObject data in obstacleList)
    //            {
    //                Gizmos.DrawCube(GetGridCellCenter(GetGridIndex(data.transform.position)), cellSize);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// Draw the debug grid lines in the rows and columns order
    /// </summary>
    //public void DebugDrawGrid(Vector3 origin, int numRows, int numCols, float cellSize, Color color)
    //{
    //    float width = (numCols * cellSize);
    //    float height = (numRows * cellSize);

    //    // Draw the horizontal grid lines
    //    for (int i = 0; i < numRows + 1; i++)
    //    {
    //        Vector3 startPos = origin + i * cellSize * new Vector3(0.0f, 0.0f, 1.0f);
    //        Vector3 endPos = startPos + width * new Vector3(1.0f, 0.0f, 0.0f);
    //        Debug.DrawLine(startPos, endPos, color);
    //    }

    //    // Draw the vertial grid lines
    //    for (int i = 0; i < numCols + 1; i++)
    //    {
    //        Vector3 startPos = origin + i * cellSize * new Vector3(1.0f, 0.0f, 0.0f);
    //        Vector3 endPos = startPos + height * new Vector3(0.0f, 0.0f, 1.0f);
    //        Debug.DrawLine(startPos, endPos, color);
    //    }
    //}
}
