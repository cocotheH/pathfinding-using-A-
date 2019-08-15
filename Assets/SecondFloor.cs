using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondFloor : MonoBehaviour
{
    public GameObject floor;
    public GameObject floor2;
    public GameObject wall;
    //public GameObject plant;
    private int width = 9;
    private int length = 30;
    private GameObject[] floors = new GameObject[270];
    private GameObject[] walls = new GameObject[100];
    //private Plant[] plants = new Plant[4];
    private int count = 0;
    private int count1 = 0;

    // Use this for initialization
    void Start()
    {
        for (int r = 0; r < length; r++)
        {
            for (int c1 = 0; c1 < 4; c1++)
            {
                floors[count] = Instantiate(floor, new Vector3(transform.position.x + r * 1f, transform.position.y, transform.position.z - c1 * 1f), Quaternion.identity) as GameObject;
                floors[count].name = "Floor " + r + "," + c1;
                floors[count].transform.Rotate(Vector3.right, 90f);
                count++;
            }
            for (int c2 = 4; c2 < width; c2++)
            {
                floors[count] = Instantiate(floor2, new Vector3(transform.position.x + r * 1f, transform.position.y, transform.position.z - c2 * 1f), Quaternion.identity) as GameObject;
                floors[count].name = "Floor " + r + "," + c2;
                floors[count].transform.Rotate(Vector3.right, 90f);
                count++;
            }
        }

        for (int r = 0; r <= 6; r++)
        {
            for (int c = 4; c < width; c++)
            {
                walls[count1] = Instantiate(wall, new Vector3(transform.position.x - 0.5f + 5.0f * r, transform.position.y + 1.5f, transform.position.z - c * 1.0f), Quaternion.identity) as GameObject;
                walls[count1].name = "Wall " + "0" + "," + c;
                walls[count1].transform.Rotate(Vector3.up, 90f);
                count1++;
            }
        }

        for (int r = 0; r < length; r++)
        {
            walls[count1] = Instantiate(wall, new Vector3(transform.position.x + r * 1.0f, transform.position.y + 1.5f, transform.position.z - 8.5f), Quaternion.identity) as GameObject;
            walls[count1].name = "Wall " + r + "," + 9;
            count1++;
        }

        for (int r = 0; r < length; r++)
        {
            if (r != 2 && r != 7 && r != 12 && r != 17 && r != 22 && r != 27)
            {
                walls[count1] = Instantiate(wall, new Vector3(transform.position.x + r * 1.0f, transform.position.y + 1.5f, transform.position.z - 3.5f), Quaternion.identity) as GameObject;
                walls[count1].name = "Wall " + r + "," + 4;
                count1++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
