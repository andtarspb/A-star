using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [HideInInspector]
    public Vector2 gridWorldSize;  // world size of the grid
    
    [SerializeField]
    int gridDimXY;           // dimensions of the grid

    float nodeRadius;       // radius of a single node

    Node[,] grid;           // grid

    public List<Node> path; // path from start to finish

    PathFinding pf;

    void Start()
    {
        gridWorldSize = new Vector2(10, 10);    // create a frame 10x10 to carry the grid

        pf = FindObjectOfType<PathFinding>();

        CreateGrid();
    }   

    public void CreateGrid()
    {
        pf.ResetStartEnd();

        // calculate radius and diameter according to dimesions
        float minGridWorldSize = (gridWorldSize.x <= gridWorldSize.y) ? gridWorldSize.x : gridWorldSize.y;
        int maxDim = (gridDimXY >= gridDimXY) ? gridDimXY : gridDimXY;
        nodeRadius = minGridWorldSize / (maxDim * 2);
        float nodeDiameter = nodeRadius * 2;

        // create the grid
        grid = new Node[gridDimXY, gridDimXY];

        // obtain coordnates of the borrom left corner
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y/2;

        // build the grid
        for (int x = 0; x < gridDimXY; x++)
        {
            for (int y = 0; y < gridDimXY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                grid[x, y] = new Node(true, worldPoint, x, y);

                //Debug.Log("node[" + x + ", " + y + "] pos: " + worldPoint);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // search for the neighbouring nodes
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)   // skip the current node                
                    continue;

                // get node's coordinates
                int checkX = node.gridX + x; 
                int checkY = node.gridY + y;

                // check if its inside the grid
                if (checkX >= 0 && checkX < gridDimXY && checkY >= 0 && checkY < gridDimXY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridDimXY - 1) * percentX);
        int y = Mathf.RoundToInt((gridDimXY - 1) * percentY);

        return grid[x, y];
    }

    public void ClearMarkings()
    {
        foreach (Node n in grid)
        {
            n.closed = false;
        }

        if (path != null)
        {
            path.Clear();
        }
    }

    public void ResetStart(Node newStart)
    {
        foreach (Node n in grid)        
            if (n != newStart)            
                n.start = false;   
    }

    public void ResetEnd(Node newEnd)
    {
        foreach (Node n in grid)
            if (n != newEnd)
                n.end = false;
    }

    void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));    // draw grid boundaries

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                // draw walkable nodes in white
                Gizmos.color = Color.white;
                // draw nodes from the closed set in red
                if (n.closed)                
                    Gizmos.color = Color.red;
                // draw obstacle nodes in black
                if (!n.walkable)
                    Gizmos.color = Color.black;
                // draw the path yellow
                if (path != null)
                    if (path.Contains(n))
                        Gizmos.color = Color.yellow;
                // draw starting point in green
                if (n.start)
                    Gizmos.color = Color.green;
                // draw end point in blue
                if (n.end)
                    Gizmos.color = Color.blue;                
                                    
                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeRadius * 2 / 1.1f) );
            }
        }
    }

}
