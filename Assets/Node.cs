using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool closed;         // wheanther a node is a part of the closed set
    public bool start;          // wheather a node is the start point
    public bool end;            // wheather a node is the end point

    public bool walkable;       // wheather a node is walkable
    public Vector3 worldPos;    // world position of a node

    public int gridX;           // x and y positions in the grid
    public int gridY;

    public int gCost;           // costs 
    public int hCost;

    public Node parent; 

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get{
            return gCost + hCost;
        }
    }
}
