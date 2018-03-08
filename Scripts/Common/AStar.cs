using UnityEngine;
using System.Collections;

public class AStar
{
    #region List fields

    #endregion

    /// <summary>
    /// Calculate the final path in the path finding
    /// </summary>
    private static ArrayList CalculatePath(Node node)
    {
        ArrayList list = new ArrayList();
        while (node != null)
        {
            list.Add(node);
            node = node.parent;
        }
        list.Reverse();
        return list;
    }

    /// <summary>
    /// Calculate the estimated Heuristic cost to the goal
    /// </summary>
    private static float HeuristicEstimateCost(Node curNode, Node goalNode)
    {
        Vector3 vecCost = curNode.position - goalNode.position;
        return vecCost.magnitude;
    }

    /// <summary>
    /// Find the path between start node and goal node using AStar Algorithm
    /// start到goal的最短路径
    /// </summary>
    public static ArrayList FindPath(Node start, Node goal)
    { 
        //Start Finding the path
        PriorityQueue closedList, openList;
        openList = new PriorityQueue();
        openList.PushAndSort(start);
        start.nodeTotalCost = 0.0f;
        start.estimatedCost = HeuristicEstimateCost(start, goal);

        closedList = new PriorityQueue();
        Node node = null;

        while (openList.Length != 0)
        {
            node = openList.First();

            if (node.position == goal.position)
            {
                return CalculatePath(node);
            }

            ArrayList neighbours = new ArrayList();
            GridManager.instance.GetNeighbours(node, neighbours);

            #region CheckNeighbours

            //Get the Neighbours
            for (int i = 0; i < neighbours.Count; i++)
            {
                //Cost between neighbour nodes
                Node neighbourNode = (Node)neighbours[i];

                if (!closedList.Contains(neighbourNode))
                {
                    //Cost from current node to this neighbour node
                    float cost = HeuristicEstimateCost(node, neighbourNode);

                    //Total Cost So Far from start to this neighbour node
                    float totalCost = node.nodeTotalCost + cost;

                    //Estimated cost for neighbour node to the goal
                    float neighbourNodeEstCost = HeuristicEstimateCost(neighbourNode, goal);

                    //Assign neighbour node properties
                    neighbourNode.nodeTotalCost = totalCost;
                    neighbourNode.parent = node;
                    neighbourNode.estimatedCost = totalCost + neighbourNodeEstCost;

                    //Add the neighbour node to the list if not already existed in the list
                    if (!openList.Contains(neighbourNode))
                    {
                        openList.PushAndSort(neighbourNode);
                    }
                }
            }

            #endregion

            closedList.Push(node);
            openList.RemoveAndSort(node);
        }

        //If finished looping and cannot find the goal then return null
        if (node.position != goal.position)
        {
            Debug.Log("no way to move");
            return null;
        }

        //Calculate the path based on the final node
        return CalculatePath(node);
    }

    //以start为中心，maxDistance范围内的所有点
    public static ArrayList checkDistance(Node start,int maxDistance)
    {
        int distance = 0;//距离计数器
        start.distance = distance;

        PriorityQueue closedList, openList;
        ArrayList nodes = new ArrayList();//返回的node列表       

        openList = new PriorityQueue();
        openList.Push(start);//起始点入队
        nodes.Add(start);

        closedList = new PriorityQueue();
        Node node = null;

        while (openList.Length != 0)
        {
            node = openList.First();

            //Debug.Log(node.position + "+" + node.distance);

            ArrayList neighbours = new ArrayList();
            GridManager.instance.GetNeighbours(node, neighbours);

            #region CheckNeighbours
            //Get the Neighbours
            for (int i = 0; i < neighbours.Count; i++)
            {
                //Cost between neighbour nodes
                Node neighbourNode = (Node)neighbours[i];

                if (neighbourNode.position == start.position)
                    continue;

                if (!closedList.Contains(neighbourNode))
                {
                    if (!openList.Contains(neighbourNode) && !nodes.Contains(neighbourNode)) 
                    {
                        neighbourNode.distance = node.distance + 1;
                        distance = neighbourNode.distance;
                        if (distance > maxDistance)
                        {
                            return nodes;
                        }
                        openList.Push(neighbourNode);
                        //Debug.Log(node.position+ "+" +neighbourNode.position + "+" + distance);
                        nodes.Add(neighbourNode);
                    }
                }
            }
            #endregion
            closedList.Push(node);
            openList.Remove(node);
        }
        return nodes;
    }
    public static ArrayList checkDistanceIncludeObstacle(Node start, int maxDistance)
    {
        int distance = 0;
        start.distance = distance;

        PriorityQueue closedList, openList;
        ArrayList nodes = new ArrayList();

        openList = new PriorityQueue();
        openList.Push(start);
        nodes.Add(start);

        closedList = new PriorityQueue();
        Node node = null;

        while (openList.Length != 0)
        {
            node = openList.First();

            //Debug.Log(node.position + "+" + node.distance);

            ArrayList neighbours = new ArrayList();
            GridManager.instance.GetNeighboursIncludeObstacle(node, neighbours);

            #region CheckNeighbours
            //Get the Neighbours
            for (int i = 0; i < neighbours.Count; i++)
            {
                //Cost between neighbour nodes
                Node neighbourNode = (Node)neighbours[i];

                if (neighbourNode.position == start.position)
                    continue;

                if (!closedList.Contains(neighbourNode))
                {
                    if (!openList.Contains(neighbourNode) && !nodes.Contains(neighbourNode))
                    {
                        neighbourNode.distance = node.distance + 1;
                        distance = neighbourNode.distance;
                        if (distance > maxDistance)
                        {
                            return nodes;
                        }
                        openList.Push(neighbourNode);
                        //Debug.Log(node.position+ "+" +neighbourNode.position + "+" + distance);
                        nodes.Add(neighbourNode);
                    }
                }
            }
            #endregion
            closedList.Push(node);
            openList.Remove(node);
        }
        return nodes;
    }
}
