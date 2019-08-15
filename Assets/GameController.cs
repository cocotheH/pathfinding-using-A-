using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject layer;
    public GameObject layero;
    public GameObject plant;
    public GameObject stair;
    public GameObject Shopper;
    public GameObject floor;

    public int stairlength = 5;
    public int population = 15;
    public int depth = 4;

    //public static int time = depth;
    private GameObject layer1;
    private GameObject layer2;
    private GameObject[] stairs = new GameObject[80];
    private GameObject[] endnodes = new GameObject[100];
    private Plant[] plants = new Plant[4];
    private Buyer[] shoppers = new Buyer[100];
    private MapNode[,,] map = new MapNode[30, 100, 100];
    private int nextUpdate = 1;
    private int count = 0;
    private int iterator = 0;
    // Use this for initialization
    void Start () {
        setUpTheMall();
        InitializeMap();
        setUpBuyers();
        for (int NofS = 0; NofS < population; NofS++) {
            //------------------COLOR THE DESTINATION ACCORDING TO SHOPPER COLOR-------------------------------//
            endnodes[NofS] = Instantiate(floor, new Vector3(shoppers[NofS].destination.X, 0.01f, shoppers[NofS].destination.Y), Quaternion.identity) as GameObject;
            endnodes[NofS].GetComponent<Renderer>().material.color = shoppers[NofS].color;
            endnodes[NofS].name = "endnodes " + NofS;
            endnodes[NofS].transform.Rotate(Vector3.right, 90f);
            //-------------------------------PATHFIND-----------------------------------------------//
            shoppers[NofS].silvernode = SilverPathNode(map[shoppers[NofS].start.r, shoppers[NofS].start.c, 0], map[shoppers[NofS].destination.r, shoppers[NofS].destination.c, 0], map, depth);
            shoppers[NofS].tmpPath = PathTrace(shoppers[NofS].silvernode, shoppers[NofS].start);
            shoppers[NofS].optimalPath = PathFinding(shoppers[NofS].tmpPath, map, depth, shoppers[NofS].start, shoppers[NofS].destination);
            shoppers[NofS].current = shoppers[NofS].start;
            map[shoppers[NofS].current.r, shoppers[NofS].current.c, 0].walkable = false;
            map[shoppers[NofS].current.r, shoppers[NofS].current.c, 1].walkable = false;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //shopper walk every second
        if (Time.time >= nextUpdate)
        {
            // Change the next update (current second+1)
            nextUpdate = Mathf.FloorToInt(Time.time) + 1;
            // follow optimal path for each shopper if at desitination reset when next reservation table start
            for (int NofS = 0; NofS < population; NofS++) {
                if (!(shoppers[NofS].current.X == shoppers[NofS].destination.X &&
                        shoppers[NofS].current.Y == shoppers[NofS].destination.Y))
                {
                    shoppers[NofS].timestep += 1;
                    if (shoppers[NofS].optimalPath != null && (iterator)< shoppers[NofS].optimalPath.Count)
                    {
                        shoppers[NofS].obj.transform.position = new Vector3(shoppers[NofS].optimalPath[iterator].X, 0.85f, shoppers[NofS].optimalPath[iterator].Y);
                        shoppers[NofS].current = shoppers[NofS].optimalPath[iterator];
                    }
                }
                else {
                    print(NofS + " " + shoppers[NofS].timestep);
                    map[shoppers[NofS].current.r, shoppers[NofS].current.c, iterator].walkable = false;
                    map[shoppers[NofS].current.r, shoppers[NofS].current.c, iterator + 1].walkable = false;
                }
            }
            
            //at half depth recalculate path restart pathfinding
            iterator++;
            if (iterator == Mathf.FloorToInt(depth / 2))
            {
                iterator = 0;
                RestReservationMap();
                for (int NofS = 0; NofS < population; NofS++) {
                    map[shoppers[NofS].current.r, shoppers[NofS].current.c, iterator].walkable = false;
                    if ((shoppers[NofS].current.X == shoppers[NofS].destination.X &&
                        shoppers[NofS].current.Y == shoppers[NofS].destination.Y) || shoppers[NofS].optimalPath == null)
                    {
                        Destroy(GameObject.Find("endnodes " + NofS));
                        shoppers[NofS].destination = findDestination(shoppers[NofS].current.r, shoppers[NofS].current.c);
                        endnodes[NofS] = Instantiate(floor, new Vector3(shoppers[NofS].destination.X, 0.01f, shoppers[NofS].destination.Y), Quaternion.identity) as GameObject;
                        endnodes[NofS].GetComponent<Renderer>().material.color = shoppers[NofS].color;
                        endnodes[NofS].name = "endnodes " + NofS;
                        endnodes[NofS].transform.Rotate(Vector3.right, 90f);
                    }
                    shoppers[NofS].silvernode = SilverPathNode(map[shoppers[NofS].current.r, shoppers[NofS].current.c, 0], map[shoppers[NofS].destination.r, shoppers[NofS].destination.c, 0], map, depth);
                    shoppers[NofS].tmpPath = PathTrace(shoppers[NofS].silvernode, shoppers[NofS].current);
                    shoppers[NofS].optimalPath = PathFinding(shoppers[NofS].tmpPath, map, depth, shoppers[NofS].current, shoppers[NofS].destination);
                }
            }
        }
    }

    private void setUpTheMall() {
        layer1 = Instantiate(layer, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        layer1.name = "layer1";
        for (int i = 0; i < stairlength; i++)
        {
            stairs[count] = Instantiate(stair, new Vector3(7f, 0f, -1f - i * 1.0f), Quaternion.identity) as GameObject;
            stairs[count].transform.Rotate(Vector3.right, 90f);
            stairs[count].name = "stair" + count;
            count += 1;
            stairs[count] = Instantiate(stair, new Vector3(12f, 0f, -1f - i * 1.0f), Quaternion.identity) as GameObject;
            stairs[count].transform.Rotate(Vector3.right, 90f);
            stairs[count].name = "stair" + count;
            count += 1;
            stairs[count] = Instantiate(stair, new Vector3(17f, 0f, -1f - i * 1.0f), Quaternion.identity) as GameObject;
            stairs[count].transform.Rotate(Vector3.right, 90f);
            stairs[count].name = "stair" + count;
            count += 1;
            stairs[count] = Instantiate(stair, new Vector3(22f, 0f, -1f - i * 1.0f), Quaternion.identity) as GameObject;
            stairs[count].transform.Rotate(Vector3.right, 90f);
            stairs[count].name = "stair" + count;
            count += 1;
        }

        layer2 = Instantiate(layero, new Vector3(0f, 0f, 0f - 1.0f * (stairlength + 1)), Quaternion.identity) as GameObject;
        layer2.name = "layer2";
        //randomly choose 4 places outside the store to plant a tree
        for (int pcount = 0; pcount < 4; pcount++)
        {
            int c, r;
            int layer = Random.Range(0, 2);
            if (layer == 0) {
                c = Random.Range(5, 9);
                r = Random.Range(0, 30);
                while ((r == 2 && c == 5) || (r == 7 && c == 5) || (r == 12 && c == 5) || (r == 17 && c == 5) || (r == 22 && c == 5) || (r == 27 && c == 5) ||
                    (r == 7 && c == 8) || (r == 12 && c == 8) || (r == 17 && c == 8) || (r == 22 && c == 8)) {
                    r = Random.Range(0, 30);
                }
                plants[pcount] = new Plant();
                plants[pcount].r = r;
                plants[pcount].c = c;
                plants[pcount].obj = Instantiate(plant, new Vector3(r * 1.0f, 0f, (8.0f-c) * 1.0f), Quaternion.identity) as GameObject;
                plants[pcount].obj.name = "plant " + pcount;
            }
            else
            {
                c = Random.Range(9+stairlength, 13+stairlength);
                r = Random.Range(0, 30);
                while ((r == 2 && c == 12 + stairlength) || (r == 7 && c == 12 + stairlength) || (r == 12 && c == 12 + stairlength) || (r == 17 && c == 12 + stairlength) || (r == 22 && c == 12 + stairlength) || (r == 27 && c == 12 + stairlength) ||
                    (r == 7 && c == 9 + stairlength) || (r == 12 && c == 9 + stairlength) || (r == 17 && c == 9 + stairlength) || (r == 22 && c == 9 + stairlength))
                {
                    r = Random.Range(0, 30);
                }
                plants[pcount] = new Plant();
                plants[pcount].r = r;
                plants[pcount].c = c;
                plants[pcount].obj = Instantiate(plant, new Vector3(r * 1.0f, 0f, (8.0f - c) * 1.0f), Quaternion.identity) as GameObject;
                plants[pcount].obj.name = "plant " + pcount;
            }
        }
    }
    //---------------------------------------------initialize reservation map based on the mall construction----------------------------------------------
    private void InitializeMap() {
        for (int r = 0; r < 30; r++) {
            for (int c = 0; c < 9; c++) {
                for (int t = 0; t < depth; t++) {
                    map[r, c, t] = new MapNode();
                }
                map[r, c, 0].X = r;
                map[r, c, 0].Y = 8.0f - c;
                map[r, c, 0].r = r;
                map[r, c, 0].c = c;
                if ((r == plants[0].r && c == plants[0].c) || (r == plants[1].r && c == plants[1].c)
                    || (r == plants[2].r && c == plants[2].c) || (r == plants[3].r && c == plants[3].c)) {
                    for (int t = 0; t < depth; t++) {
                        map[r, c, t].walkable = false;
                    }
                }
                else
                {
                    for (int t = 0; t < depth; t++)
                    {
                        map[r, c, t].walkable = true;
                    }
                }

            }
        }
        for (int r = 7; r < 27; r = r + 5)
        {
            for (int c = 9; c < 9+stairlength; c++)
            {
                for (int t = 0; t < depth; t++)
                {
                    map[r, c, t] = new MapNode();
                }
                map[r, c, 0].X = r;
                map[r, c, 0].Y = 8.0f - c;
                map[r, c, 0].r = r;
                map[r, c, 0].c = c;
                for (int t = 0; t < depth; t++)
                {
                    map[r, c, t].walkable = true;
                }
            }
        }

        for (int r = 0; r < 30; r++)
        {
            for (int c = 9 + stairlength; c < 18 + stairlength; c++)
            {
                for (int t = 0; t < depth; t++)
                {
                    map[r, c, t] = new MapNode();
                }
                map[r, c, 0].X = r;
                map[r, c, 0].Y = 8.0f - c;
                map[r, c, 0].r = r;
                map[r, c, 0].c = c;
                if ((r == plants[0].r && c == plants[0].c) || (r == plants[1].r && c == plants[1].c)
                    || (r == plants[2].r && c == plants[2].c) || (r == plants[3].r && c == plants[3].c))
                {
                    for (int t = 0; t < depth; t++)
                    {
                        map[r, c, t].walkable = false;
                    }
                }
                else {
                    for (int t = 0; t < depth; t++)
                    {
                        map[r, c, t].walkable = true;
                    }
                }
            }
        }

        //------------------------------set up the neighbours for each grid---------------------------------------
        for (int c = 0; c < 18 + stairlength; c++) {
            for (int r = 0; r < 30; r++) {
                if (c == 0)
                {
                    if (r == 0 || r == 5 || r == 10 || r == 15 || r == 20 || r == 25)
                    {
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                    else if (r == 4 || r == 9 || r == 14 || r == 19 || r == 24 || r == 29)
                    {
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                    else {
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                }
                else if (c == 1 || c == 2 || c == 3 || c == 14 + stairlength || c == 15 + stairlength || c == 16 + stairlength)
                {
                    if (r == 0 || r == 5 || r == 10 || r == 15 || r == 20 || r == 25)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                    else if (r == 4 || r == 9 || r == 14 || r == 19 || r == 24 || r == 29)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                    else {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                }
                else if (c == 4 || c == 12 + stairlength)
                {
                    if (r == 2 || r == 7 || r == 12 || r == 17 || r == 22 || r == 27)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                    else if (r == 0 || r == 5 || r == 10 || r == 15 || r == 20 || r == 25)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                        if (c == 12 + stairlength &&  r != 0) {
                            map[r, c, 0].West = map[r - 1, c, 0];
                        }
                    }
                    else if (r == 4 || r == 9 || r == 14 || r == 19 || r == 24 || r == 29)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        if (c == 12 + stairlength && r != 29){
                            map[r, c, 0].East = map[r + 1, c, 0];
                        }
                    }
                    else {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                    }
                }
                else if (c == 5 || c == 13 + stairlength)
                {
                    if (r == 2 || r == 7 || r == 12 || r == 17 || r == 22 || r == 27)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                    else if (r == 0 || r == 5 || r == 10 || r == 15 || r == 20 || r == 25)
                    {
                        map[r, c, 0].South = map[r, c + 1, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                        if (c == 5 && r != 0) {
                            map[r, c, 0].West = map[r - 1, c, 0];
                        }
                    }
                    else if (r == 4 || r == 9 || r == 14 || r == 19 || r == 24 || r == 29)
                    {
                        map[r, c, 0].South = map[r, c + 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        if (c == 5 && r != 29)
                        {
                            map[r, c, 0].East = map[r + 1, c, 0];
                        }
                    }
                    else {
                        map[r, c, 0].South = map[r, c + 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                    }
                }
                else if (c == 6 || c == 7 || c == 10 + stairlength || c == 11 + stairlength)
                {
                    if (r == 0)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                    else if (r == 29)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                    else {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                }
                else if (c == 8)
                {
                    if (r == 0)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                    }
                    else if (r == 29)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                    }
                    else if (r == 2 || r == 7 || r == 12 || r == 17 || r == 22 || r == 27)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                    else {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                    }
                }
                else if (c == 9 + stairlength)
                {
                    if (r == 0)
                    {
                        map[r, c, 0].South = map[r, c + 1, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                    }
                    else if (r == 29)
                    {
                        map[r, c, 0].South = map[r, c + 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                    }
                    else if (r == 2 || r == 7 || r == 12 || r == 17 || r == 22 || r == 27)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                    else {
                        map[r, c, 0].South = map[r, c + 1, 0];
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                    }
                }
                else if (c == 17 + stairlength) {
                    if (r == 0 || r == 5 || r == 10 || r == 15 || r == 20 || r == 25)
                    {
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].North = map[r, c - 1, 0];
                    }
                    else if (r == 4 || r == 9 || r == 14 || r == 19 || r == 24 || r == 29)
                    {
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].North = map[r, c - 1, 0];
                    }
                    else {
                        map[r, c, 0].East = map[r + 1, c, 0];
                        map[r, c, 0].West = map[r - 1, c, 0];
                        map[r, c, 0].North = map[r, c - 1, 0];
                    }
                }
                else if(c>=9 && c < 9+stairlength) {
                    if (r == 7 || r == 12 || r == 17 || r == 22)
                    {
                        map[r, c, 0].North = map[r, c - 1, 0];
                        map[r, c, 0].South = map[r, c + 1, 0];
                    }
                }
            }
        }

    }
    //---------------------------------------give an initial start point and end point to shoppers----------------------
    private void setUpBuyers()
    {
        int red = Random.Range(0, 256);
        int green = Random.Range(0, 256);
        int blue = Random.Range(0, 256);
        for (int i = 0; i < population; i++)
        {
            int c = Random.Range(0,18+stairlength);
            int r = Random.Range(0, 30);
            while (map[r, c, 0] == null || (map[r, c, 0] != null && map[r, c, 0].walkable == false)) {
                    c = Random.Range(0, 18 + stairlength);
                    r = Random.Range(0, 30);
            }
            shoppers[i] = new Buyer();
            shoppers[i].obj = Instantiate(Shopper, new Vector3(map[r, c, 0].X, 0.85f, map[r, c, 0].Y), Quaternion.identity) as GameObject;
            shoppers[i].color = new Color(red/255f, green/ 255f, blue / 255f);
            shoppers[i].obj.GetComponent<Renderer>().material.color = shoppers[i].color;
            shoppers[i].obj.name = "Buyers " + i;
            map[r, c, 0].walkable = false;
            shoppers[i].start = map[r, c, 0];
            shoppers[i].destination = findDestination(r, c);
            red = Random.Range(0, 256);
            green = Random.Range(0, 256);
            blue = Random.Range(0, 256);
}
    }

    private MapNode findDestination(int startR, int startC) {
        MapNode destination = new MapNode();
        int actionMode = Random.Range(0, 2);
        //move option
        if (actionMode == 0) {
            int c = Random.Range(5, 13 + stairlength);
            int r = Random.Range(0, 30);
            while (map[r, c, 0] == null || (map[r, c, 0] != null && map[r, c, 0].walkable == false) || ( c>=9  && c < 9+stairlength ))
            {
                c = Random.Range(5, 13 + stairlength);
                r = Random.Range(0, 30);
            }
            destination.r = r;
            destination.c = c;
            destination.X = map[r, c, 0].X;
            destination.Y = map[r, c, 0].Y;
        }
        //Shop option
        if (actionMode == 1) {
            int curStoreN = getStoreNumber(startR, startC);
            int DStoreN = chooseStore(curStoreN);
            if (DStoreN <= 5) {
                int c = Random.Range(0,5);
                int r = Random.Range(DStoreN * 5, DStoreN * 5 + 5);
                while (map[r, c, 0] == null || (map[r, c, 0] != null && map[r, c, 0].walkable == false))
                {
                    c = Random.Range(0, 5);
                    r = Random.Range(DStoreN * 5, DStoreN * 5 + 5);
                }
                destination.r = r;
                destination.c = c;
                destination.X = map[r, c, 0].X;
                destination.Y = map[r, c, 0].Y;
            }
            if (DStoreN > 5)
            {
                int c = Random.Range(13 + stairlength, 18 + stairlength);
                int r = Random.Range((DStoreN-6) * 5, (DStoreN - 6) * 5 + 5);
                while (map[r, c, 0] == null || (map[r, c, 0] != null && map[r, c, 0].walkable == false))
                {
                    c = Random.Range(13 + stairlength, 18 + stairlength);
                    r = Random.Range((DStoreN - 6) * 5, (DStoreN - 6) * 5 + 5);
                }
                destination.r = r;
                destination.c = c;
                destination.X = map[r, c, 0].X;
                destination.Y = map[r, c, 0].Y;
            }
        }
        
        return destination;
    }

    //get current store number if in a store
    private int getStoreNumber(int startR, int startC) {
        int storenumber =-1;
        if (startC <= 4 && startC >= 0) {
            if (startR <= 4 && startR >= 0) {
                storenumber = 0;
            }
            if (startR > 4 && startR >=9)
            {
                storenumber = 1;
            }
            if (startR > 9 && startR >= 14)
            {
                storenumber = 2;
            }
            if (startR > 14 && startR >= 19)
            {
                storenumber = 3;
            }
            if (startR > 19 && startR >= 24)
            {
                storenumber = 4;
            }
            if (startR > 24 && startR >= 29)
            {
                storenumber = 5;
            }
        }
        if (startC <= 17 + stairlength && startC >= 13 + stairlength)
        {
            if (startR <= 4 && startR >= 0)
            {
                storenumber = 6;
            }
            if (startR > 4 && startR >= 9)
            {
                storenumber = 7;
            }
            if (startR > 9 && startR >= 14)
            {
                storenumber = 8;
            }
            if (startR > 14 && startR >= 19)
            {
                storenumber = 9;
            }
            if (startR > 19 && startR >= 24)
            {
                storenumber = 10;
            }
            if (startR > 24 && startR >= 29)
            {
                storenumber = 11;
            }
        }
        return storenumber;
    }
    //random choose a store
    private int chooseStore(int curStoreN) {
        int storenumber = -1;
        if (curStoreN == -1) {
            storenumber = Random.Range(0, 12);
        }
        if (curStoreN == 0) {
            storenumber = Random.Range(1, 12);
        }
        for (int i = 1; i <= 11; i++){
            if (curStoreN == i) {
                storenumber = Random.Range(1, 12);
                if (storenumber == curStoreN) {
                    storenumber = 0;
                }
            }
        }

        return storenumber;
    }
    //get estimate H value
    private float AbstractDistance(MapNode p, MapNode q) {
        float distance;
        if (p == q)
        {
            return 0;
        }
        else {
            distance = System.Math.Abs(p.X - q.X) + System.Math.Abs(p.Y - q.Y);
        }
        return distance;
    }
    //initialize start point g,h,f value
    private void initialStartPoint(MapNode start, MapNode end) {
        start.G = 0;
        start.H = AbstractDistance(start, end);
        start.F = start.H + start.G;
    }

    //A* path finding as if there is only one shopper
    private MapNode SilverPathNode(MapNode start, MapNode end, MapNode[,,] map, int time)
    {
        List<MapNode> closedList = new List<MapNode>();
        List<MapNode> openList = new List<MapNode>();
        MapNode endpoint = map[end.r, end.c, 0];
        MapNode startpoint = map[start.r, start.c, 0];
        startpoint.G = 0;
        startpoint.H = AbstractDistance(startpoint, endpoint);
        startpoint.F = startpoint.G + startpoint.H;
        openList.Add(startpoint);
        MapNode currentNode = openList[0];
        while (openList.Count > 0)
        {
            currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F < currentNode.F || (openList[i].F == currentNode.F && openList[i].H < currentNode.H))
                {
                    currentNode = openList[i];
                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endpoint)
            {
                return endpoint;
            }

            foreach (MapNode neighbor in currentNode.GetNeighbors(currentNode))
            {

                if (!neighbor.walkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                float newMovementCost = currentNode.G + AbstractDistance(currentNode, neighbor);
                if (newMovementCost < neighbor.G || !openList.Contains(neighbor))
                {
                    neighbor.G = newMovementCost;
                    neighbor.H = AbstractDistance(neighbor, endpoint);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);

                    }

                }
            }
        }
        for (int r = 0; r < 30; r++)
        {
            for (int c = 0; c < 17 + stairlength; c++)
            {
                if (map[r, c, 0] != null) {
                        map[r, c, 0].F = 0;
                        map[r, c, 0].H = 0;
                        map[r, c, 0].G = 0;
                }
           }
        }
        return currentNode;
    }
    // get A* path for individual shoppers as if other shoppers are not exsited
    private List<MapNode> PathTrace(MapNode end, MapNode start) {
        end = map[end.r, end.c, 0];
        start = map[start.r, start.c, 0];
        List<MapNode> path = new List<MapNode>();
        MapNode currentPoint = end;
        while (currentPoint != start) {
            path.Add(currentPoint);
            currentPoint = currentPoint.parent;
        }
        path.Reverse();
        foreach (MapNode n in map)
        {
            if (n != null && n.parent != null) {
                n.parent = null;
            }
        }
        return path;
    }   

    //silver cooprative A* pathfinding with time duration window
    public List<MapNode> PathFinding(List<MapNode> tmpPath, MapNode[,,] map, int depth,MapNode start, MapNode end)
    {

        MapNode currentPoint = map[start.r, start.c, 0];
        MapNode terminalPoint = map[end.r, end.c, 0];
        MapNode prePoint = currentPoint;
        currentPoint.H = AbstractDistance(currentPoint, terminalPoint);

        List<MapNode> optimalPath = new List<MapNode>();
        for (int time = 0; time < depth; time++)
        {
            //to avoid head to head collision reserve sampepoint at next time
            map[currentPoint.r, currentPoint.c, time].walkable = false;
            //if find destination before duration ends break
            if ((time) > tmpPath.Count - 1)
            {
                map[currentPoint.r, currentPoint.c, time].walkable = false;
            }

            else {
                map[currentPoint.r, currentPoint.c, time].walkable = false;
                //--------------------get the current time position
                List<MapNode> notOptimal = new List<MapNode>();
                foreach (MapNode neighbour in currentPoint.GetNeighbors(currentPoint))
                {
                    neighbour.H = AbstractDistance(neighbour, terminalPoint);

                    //if neighbour at timt t is waikable and belong to A* path at time t add neighbour
                    if (map[neighbour.r, neighbour.c, time].walkable && tmpPath[time] == (neighbour))
                    {
                        currentPoint = neighbour;
                        notOptimal = new List<MapNode>(); 
                        break;
                    }
                    //otherwise we pick the avalible neighbour with the samllest to-destination value
                    else if (map[neighbour.r, neighbour.c, time].walkable)
                    {

                        notOptimal.Add(neighbour);
                    }
                    else if (currentPoint.X == terminalPoint.X &&
                            currentPoint.Y == terminalPoint.Y)
                    {
                        map[currentPoint.r, currentPoint.c, time].walkable = false;
                    }
                }
                if (notOptimal.Count > 0)
                {
                    //get neighbour with minimum h value if walkable
                    MapNode subOptimal = BestNodeFromList(notOptimal);
                    if (currentPoint.H > subOptimal.H)
                    {
                        currentPoint = subOptimal;
                    }
                }
                optimalPath.Add(currentPoint);
                //reserve
                map[currentPoint.r, currentPoint.c, time].walkable = false;
            }
            
        }
        return optimalPath;

    }

    // pick node with smallest to-destination value
    private MapNode BestNodeFromList(List<MapNode> ls)
    {
        MapNode best = ls[0];
        foreach (MapNode n in ls)
        {
            if (n.H < best.H)
            {
                best = n;
            }
        }
        return best;
    }

    //reset reservation map
    private void RestReservationMap() {
        InitializeMap();
        for (int i = 0; i < population; i++) {
            map[shoppers[i].current.r, shoppers[i].current.c, 0].walkable = false;
            map[shoppers[i].current.r, shoppers[i].current.c, 1].walkable = false;
        }
    }
}
