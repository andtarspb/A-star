using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    Vector3 startPos, endPos;    // coordinates of start and end points
    bool setStart;          // true when start point is setted
    bool setEnd;            // true when end point is setted

    [SerializeField]        
    int straightCost;       // cost to move straight (left, right, up, down)
    [SerializeField]
    int diagonalCost;       // cost to move diagonal (up-right, up-left, ...)

    Grid grid;              // reference to the grid

    Vector3 mousePos;       // position of a mouse

    public string mode;     // mode of the mouse click (create obstacle, set start point, ...)

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        // get mouse position
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        if (Input.GetMouseButton(0))
        {
            // when mose is within the grid (1.8 for conviniency)
            if (mousePos.x <= grid.transform.position.x + (grid.gridWorldSize.x/1.8f) && mousePos.x >= grid.transform.position.x - (grid.gridWorldSize.x / 1.8f) 
                && mousePos.y <= grid.transform.position.y + (grid.gridWorldSize.y / 1.8f) && mousePos.y >= grid.transform.position.y - (grid.gridWorldSize.y / 1.8f))
            {
                // handle mouse click
                switch (mode)
                {
                    default:
                        break;
                    case "obstacle":
                        Node nObstacle = grid.NodeFromWorldPoint(mousePos);
                        if (!nObstacle.start && !nObstacle.end) // if there the node isn't start or end point
                        {
                            nObstacle.walkable = false;
                            if (setStart && setEnd)
                                FindPath(startPos, endPos);
                        }
                        
                        break;
                    case "eraser":
                        grid.NodeFromWorldPoint(mousePos).walkable = true;
                        if (setStart && setEnd)
                            FindPath(startPos, endPos);
                        break;
                    case "start":
                        Node nStart = grid.NodeFromWorldPoint(mousePos);
                        
                        if (nStart.walkable) // if there is no obstacles - set start position
                        {
                            startPos = mousePos;
                            nStart.start = true;
                            grid.ResetStart(nStart);
                            setStart = true;
                            if (setEnd)
                                FindPath(startPos, endPos);
                        }                       
                        break;
                    case "end":
                        Node nEnd = grid.NodeFromWorldPoint(mousePos);
                        
                        if (nEnd.walkable)  // if there is no obstacles - set end position
                        {
                            endPos = mousePos;
                            nEnd = grid.NodeFromWorldPoint(mousePos);
                            nEnd.end = true;
                            grid.ResetEnd(nEnd);
                            setEnd = true;
                            if (setStart)
                                FindPath(startPos, endPos);
                        }
                        
                        break;
                }
            }
        }
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        grid.ClearMarkings();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // create open and closed sets
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        // add a start node to the open set
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];  // current node is the first element of the open set

            // search for the node with a lowest f cost
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            // remove it from the open set
            openSet.Remove(currentNode);
            // add it to the closed set
            closedSet.Add(currentNode);
            currentNode.closed = true;

            if (currentNode == targetNode)  // path has been found
            {
                RetracePath(startNode, targetNode);
                return;
            }

            // loop through each of the neighbour nodes of the current node
            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                // if the neighbour is not traversable or in closed set - skip to the next neighbour
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue; 
                }

                // if the new path to the neighbour is shorter or neighbour is not in open set
                int newMoveCostToNeighbour = currentNode.gCost + GetDistanceBetweenNodes(currentNode, neighbour);
                if (newMoveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // set the f cost of the neighbour
                    neighbour.gCost = newMoveCostToNeighbour;
                    neighbour.hCost = GetDistanceBetweenNodes(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }

        }

    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        // build the path backwards: from the end to the beginning
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        ///path.Reverse();     // reverse the path 

        grid.path = path;
    }

    public void ResetStartEnd()
    {
        setStart = false;
        setEnd = false;
    }

    int GetDistanceBetweenNodes(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)        
            return diagonalCost * dstY + 10 * (dstX - dstY);
        else
            return diagonalCost * dstX + 10 * (dstY - dstX);

    }
}
