using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    public float X;
    public float Y;
    public int r;
    public int c;
    public float G;
    public float H;
    public float reverseG;
    public float reverseH;
    public float F;
    public bool walkable;
    public MapNode North;
    public MapNode South;
    public MapNode East;
    public MapNode West;
    public MapNode parent;

    public List<MapNode> GetNeighbors(MapNode node)
    {
        List<MapNode> neighbors = new List<MapNode>();


        if (node.North != null) {
            neighbors.Add(node.North);
        }
        if (node.South != null)
        {
            neighbors.Add(node.South);
        }
        if (node.East != null)
        {
            neighbors.Add(node.East);
        }
        if (node.West != null)
        {
            neighbors.Add(node.West);
        }
        return neighbors;

    }
}