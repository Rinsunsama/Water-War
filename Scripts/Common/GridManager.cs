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

    private Vector3 origin = new Vector3();//第一个点index = 0
    private GameObject[] obstacleList;//障碍物列表
    public GameObject[] play1_heroList;//玩家1英雄列表
    public GameObject[] play2_heroList;//玩家2英雄列表
    public Node[,] nodes { get; set; }//单元格数组
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
        CalculateObstacles();//将障碍物所在的单元格标记为是障碍物
        CalculateHero();//将英雄所在的单元格标记为是障碍物
     //   scanWaterEyesAndCalculateWaterPower();
    }

    void scanWaterEyesAndCalculateWaterPower()
    {
        GameManager.waterPower = (GameManager.roundIndex > 6) ? 10 : 5 + GameManager.roundIndex - 1;        //计算圣水量
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
    /// 根据单元格，获得其在单元格数组中的位置
    /// </summary>
    public Node CalculateAtNodes(Node node)
    {
        int nodeIndex = GetGridIndex(node.position);//节点编号
        int row = GetRow(nodeIndex);//行
        int col = GetColumn(nodeIndex);//列
        return nodes[row, col];
    }
    /// <summary>
    /// 根据单元格，判断其是否在障碍物位置
    /// </summary>
    public bool IsObstacle(Node node)
    {
        int nodeIndex = GetGridIndex(node.position);//节点编号
        int row = GetRow(nodeIndex);//行
        int col = GetColumn(nodeIndex);//列
        if (nodes[row, col].bObstacle)
            return true;
        else
            return false;
    }
    void CalculateObstacles()
    {
        nodes = new Node[numOfRows, numOfColumns];//初始化单元格数组

        //将所有单元格(node)放入单元格数组(nodes[,])
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

        //将在障碍物列表中的位置的节点，标记为有障碍物
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
    //将所有英雄所在的障碍物标志取消
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
    //将所有英雄所在的单元格标记为是障碍物
    void CalculateHero()
    {
        //在玩家1英雄列表中的位置的节点，标记为有障碍物
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

        //在玩家1英雄列表中的位置的节点，标记为有障碍物
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
    /// 根据单元格编号，获取单元格位置的中心
    /// 本函数暂时没用
    /// </summary>
    public Vector3 GetGridCellCenter(int index)
    {
        Vector3 cellPosition = GetGridCellPosition(index);
        cellPosition.x += (gridCellSize / 2.0f);
        cellPosition.z += (gridCellSize / 2.0f);

        return cellPosition;
    }

    /// <summary>
    /// 根据单元格编号，获取单元格位置
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
    /// 根据单元格位置，获得该位置单元格编号
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
    /// 根据编号，获得单元格的行row
    /// </summary>
    public int GetRow(int index)
    {
        int row = index / numOfColumns;
        return row;
    }

    /// <summary>
    /// 根据编号，获得单元格的列col
    /// </summary>
    public int GetColumn(int index)
    {
        int col = index % numOfColumns;
        return col;
    }

    /// <summary>
    /// 判断位置是否在范围(地图)内
    /// </summary>
    public bool IsInBounds(Vector3 pos)
    {
        float width = numOfColumns * gridCellSize;
        float height = numOfRows * gridCellSize;

        return (pos.x >= Origin.x && pos.x <= Origin.x + width && pos.x <= Origin.z + height && pos.z >= Origin.z);
    }

    /// <summary>
    /// 对于给定的node，获取它的四个方向可达的邻居结点
    /// </summary>
    public void GetNeighbours(Node node, ArrayList neighbors)
    {
        Vector3 neighborPos = node.position;
        int neighborIndex = GetGridIndex(neighborPos);

        int row = GetRow(neighborIndex);
        int column = GetColumn(neighborIndex);

        //下
        int leftNodeRow = row - 1;
        int leftNodeColumn = column;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //上
        leftNodeRow = row + 1;
        leftNodeColumn = column;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //右
        leftNodeRow = row;
        leftNodeColumn = column + 1;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //左
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

        //下
        int leftNodeRow = row - 1;
        int leftNodeColumn = column;
        AssignNeighbourIncludeObstacle(leftNodeRow, leftNodeColumn, neighbors);

        //上
        leftNodeRow = row + 1;
        leftNodeColumn = column;
        AssignNeighbourIncludeObstacle(leftNodeRow, leftNodeColumn, neighbors);

        //右
        leftNodeRow = row;
        leftNodeColumn = column + 1;
        AssignNeighbourIncludeObstacle(leftNodeRow, leftNodeColumn, neighbors);

        //左
        leftNodeRow = row;
        leftNodeColumn = column - 1;
        AssignNeighbourIncludeObstacle(leftNodeRow, leftNodeColumn, neighbors);
    }

    /// <summary>
    /// 检查节点：行列没有超过范围，并且没有被标记为障碍物
    /// 然后将节点放入邻居节点列表neighbors
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
