using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buyer{
    public MapNode start;
    public MapNode current;
    public MapNode destination;
    public GameObject obj;
    public MapNode silvernode;
    public List<MapNode> tmpPath;
    public List<MapNode> optimalPath;
    public Color color;
    public int timestep = 0;
}
