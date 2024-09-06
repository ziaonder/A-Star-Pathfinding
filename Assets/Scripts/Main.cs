// Code by: ziaonder
// Matrix indexes has 3 values. 1 => path, 0 => empty, -1 => obstacle.

using System.Collections.Generic;
using UnityEngine;
using System;

public enum Type
{
    START,
    GOAL,
    PATH
}

public class Node
{
    public int x, y;
    public int gCost, hCost;
    public Node parent;
    public int FCost => gCost + hCost;

    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        gCost = int.MaxValue;
        hCost = 0;
        parent = null;
    }


    public override bool Equals(object obj)
    {
        if (obj is Node other)
        {
            return x == other.x && y == other.y;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return (x, y).GetHashCode();
    }
}

public class Main : MonoBehaviour
{
    public static int[,] matrix;
    private List<Node> openList, closedList;
    public static int width = 10, height = 10;
    private int startX = 0, startY = 0, goalX = 9, goalY = 9;
    private Vector2 canvasPos;
    public static event Action OnCalculation;

    private void Awake()
    {
        openList = new List<Node>();
        closedList = new List<Node>();
        InitializeMatrix(height, width);
    }

    private void OnEnable()
    {
        Matrix.OnMouseButtonDown += AssignNodeValue;
    }

    private void OnDisable()
    {
        Matrix.OnMouseButtonDown -= AssignNodeValue;
    }

    public void OnFind()
    {
        openList.Clear();
        closedList.Clear();
        FindPath();
        InitializeMatrix(height, width);
    }

    private void AssignNodeValue(Vector2 currentStart, Vector2 currentGoal)
    {
        startX = (int)currentStart.x;
        startY = (int)currentStart.y;
        goalX = (int)currentGoal.x;
        goalY = (int)currentGoal.y;
    }

    private void FindPath()
    {
        Node startingNode = new Node(startX, startY);
        Node currentNode = null;
        startingNode.x = startX;
        startingNode.y = startY;
        startingNode.gCost = 0;
        startingNode.hCost = CalculateManhattan(startingNode, goalX, goalY);
        startingNode.parent = null;
        openList.Add(startingNode);
        while (openList.Count > 0 && (currentNode == null || (currentNode.x != goalX || currentNode.y != goalY)))
        {
            // Select the node with the lowest F cost.
            foreach (Node node in openList)
            {
                if (currentNode == null || 
                    node.FCost < currentNode.FCost || 
                    (node.FCost == currentNode.FCost && node.hCost < currentNode.hCost))
                {
                    currentNode = node;
                }
            }

            // Remove the lowest F cost node from open list and add it to closed list.
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Check if the current node is the goal.
            if (currentNode.x == goalX && currentNode.y == goalY)
            {
                Debug.Log("Goal Found!");
                while(currentNode != null)
                {
                    matrix[currentNode.y, currentNode.x] = 1;
                    currentNode = currentNode.parent;
                }

                OnCalculation?.Invoke();
                break;
            }

            // Get the neighbors of the current node.
            List<Node> neighbors = GetNeighbors(currentNode);

            foreach (Node neighbor in neighbors)
            {
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                int newGCost = currentNode.gCost + 1;
                if (newGCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = CalculateManhattan(neighbor, goalX, goalY);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
            currentNode = null;
        }
    }

    private int CalculateManhattan(Node current, int goalX, int goalY)
    {
        return Mathf.Abs(current.x - goalX) + Mathf.Abs(current.y - goalY);
    }

    private void InitializeMatrix(int rows, int cols)
    {
        if (matrix == null)
            matrix = new int[rows, cols];
        else
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == -1)
                        continue;

                    matrix[i, j] = 0;
                }
            }
        }
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int[,] movements = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
        for (int i = 0; i < 4; i++)
        {
            int x = node.x + movements[i, 0];
            int y = node.y + movements[i, 1];
            if (x >= 0 && x < matrix.GetLength(1) && y >= 0 && y < matrix.GetLength(0))
            {
                // -1 means obstacle.
                if (matrix[y, x] == -1)
                {
                    continue;
                }

                Node neighbor = new Node(x, y);
                //neighbor.x = x;
                //neighbor.y = y;
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}
