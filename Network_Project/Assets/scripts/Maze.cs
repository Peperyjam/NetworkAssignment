using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using Random = UnityEngine.Random;

public class Maze : NetworkBehaviour
{
    public static Maze Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public class Node
    {
        public Node(float Y, float X)
        {
            x = X;
            y = Y;
        }
        public float x, y;
        public int cost;
        public Dictionary<string, Node> fourneighbours = new Dictionary<string, Node>();
        public Dictionary<string, Wall> walls = new Dictionary<string, Wall>();
        public List<Node> openNeighbours = new List<Node>();
        public GameObject body;
        public string cameFrom;
        public bool isActive;

    }
    public class Wall
    {
        public bool isStanding = true;
        public bool isEdge = false;
        public GameObject body;
        public int spawnedOrder;


    }

    public Stack<Node> visited = new Stack<Node>();
    public Stack<Node> goBack = new Stack<Node>();

    [SerializeField] GameObject nodeObj;
    [SerializeField] GameObject wallObj;

    int wallNumber;

    Dictionary<int,Wall> wallDict = new Dictionary<int,Wall>();
    public Node[,] maze;
    public int width;
    public int height;
    float halfNode = 0.5f;
    public float dificulty;

    public void Backtrack()
    {
        Node current = maze[Random.Range(0,height-1), Random.Range(0,width-1)];
        goBack.Push(current);
        while (goBack.Count > 0)
        {
            visited.Push(current);
            foreach (KeyValuePair<string, Node> pair in current.fourneighbours)
            {
                if (pair.Value != null && !visited.Contains(pair.Value))
                {
                    pair.Value.cameFrom = OpositeDir(pair.Key);
                    goBack.Push(pair.Value);
                }
            }
            current = goBack.Pop();
            if (current.cameFrom != null)
            {
                FellRpc(current.walls[current.cameFrom].spawnedOrder);
                current.openNeighbours.Add(current.fourneighbours[current.cameFrom]);
                current.fourneighbours[current.cameFrom].openNeighbours.Add(current);
            }
        }
        WallBreaker();
    }
    [Rpc(SendTo.Everyone)]
    public void FellRpc(int wallNum)
    {

        wallDict[wallNum].body.SetActive(false);
        wallDict[wallNum].isStanding = false;
    }
    void WallBreaker()
    {
        foreach (Node node in maze)
        {
            foreach (KeyValuePair<string, Wall> pair in node.walls)
            {
                if(!pair.Value.isEdge)
                {
                    if (Random.Range(0, maze.Length * 4) <= (maze.Length * 4)/dificulty)
                    {
                        FellRpc(pair.Value.spawnedOrder);

                    }
                }
            }
        }
    }
    [Rpc(SendTo.Everyone)]
    public void CreateGridRpc(int x, int y)
    {
        maze = new Node[y, x];
        width = x;
        height = y;
        CreateMaze();
        CreateWalls();
    }

    // Update is called once per frame

    void CreateMaze()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                maze[y, x] = new Node(y, x);
                maze[y, x].body = Instantiate(nodeObj, new Vector2(halfNode + transform.position.x + x, halfNode + transform.position.y + y), transform.rotation);
            }

        }
        Populateneighbours();
    }
    public string OpositeDir(string dir)
    {
        if (dir == "N") return "S";
        if (dir == "S") return "N";
        if (dir == "E") return "W";
        if (dir == "W") return "E";
        else return null;
    }
    public void Populateneighbours()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                maze[y, x].fourneighbours = new Dictionary<string, Node>();

                if (x + 1 < width) maze[y, x].fourneighbours.Add("E", maze[y, x + 1]);
                if (y + 1 < height) maze[y, x].fourneighbours.Add("N", maze[y + 1, x]);
                if (y - 1 >= 0) maze[y, x].fourneighbours.Add("S", maze[y - 1, x]);
                if (x - 1 >= 0) maze[y, x].fourneighbours.Add("W", maze[y, x - 1]);
                maze[y, x].fourneighbours = shuffle(maze[y, x].fourneighbours);
            }

        }
    }
    Dictionary<string, Node> shuffle(Dictionary<string, Node> fourneighbours)
    {
        System.Random rand = new System.Random();
        return fourneighbours.OrderBy(x => rand.Next())
        .ToDictionary(item => item.Key, item => item.Value);
    }
    void CreateWalls()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Waller(y, x, "N", "S", new Vector3(halfNode + transform.position.x + x, halfNode + transform.position.y + y + halfNode, -0.1f), transform.rotation);
                Waller(y, x, "S", "N", new Vector3(halfNode + transform.position.x + x, halfNode + transform.position.y + y - halfNode, -0.1f), transform.rotation);
                Waller(y, x, "E", "W", new Vector3(halfNode + transform.position.x + x + halfNode, halfNode + transform.position.y + y, -0.1f), Quaternion.Euler(0, 0, 90));
                Waller(y, x, "W", "E", new Vector3(halfNode + transform.position.x + x - halfNode, halfNode + transform.position.y + y, -0.1f), Quaternion.Euler(0, 0, 90));
            }
        }
    }


    void Waller(int y, int x, string direction, string oposite, Vector3 vec, Quaternion rot)
    {
        if (!maze[y, x].walls.ContainsKey(direction))
        {
            GameObject obj = Instantiate(wallObj, vec, rot);
            Wall wall = new Wall();
            wall.body = obj;
            wall.spawnedOrder = wallNumber;
            wallDict[wallNumber] = wall;
            wallNumber++;
            if (maze[y, x].fourneighbours.ContainsKey(direction) && !maze[y, x].fourneighbours[direction].walls.ContainsKey(oposite))
            {
                maze[y, x].fourneighbours[direction].walls.Add(oposite, wall);
            }
            if (!maze[y, x].fourneighbours.ContainsKey(direction))
            {
                wall.isEdge = true;
            }

            maze[y, x].walls.Add(direction, wall);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector2(transform.position.x + width/2,transform.position.y + height/2), new Vector2(width,height));
        Gizmos.color = Color.green;
    }
}
