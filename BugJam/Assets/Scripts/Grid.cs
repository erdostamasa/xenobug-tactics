using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Grid : MonoBehaviour {
    public static Grid instance;

    [Header("Tiles")]
    [SerializeField] Transform tilePrefab;
    [SerializeField] Transform mountainTilePrefab;

    [Header("Setup")]
    [SerializeField] Transform coordTextPrefab;
    [SerializeField] TextAsset mapFile;


    Tile[,] grid;
    //Tile selectedTile;

    void Awake() {
        instance = this;
    }

    void Start() {
        GenerateGrid();
    }


    Node[,] nodes;
    Node origin = null;
    Node target = null;
    List<Node> toSearch;
    List<Node> processed;

    public List<Node> GetPath(Tile originTile, Tile targetTile) {
        // build graph
        nodes = BuildNodeArray();
        // print(Utils.MatrixToString(nodes));

        //find start and target
        // Node origin = null;
        // Node target = null;
        for (int x = 0; x < nodes.GetLength(0); x++) {
            for (int y = 0; y < nodes.GetLength(1); y++) {
                if (nodes[x, y] != null) {
                    if (nodes[x, y].tile == originTile) {
                        origin = nodes[x, y];
                    }
                    else if (nodes[x, y].tile == targetTile) {
                        target = nodes[x, y];
                    }
                }
            }
        }

        if (origin == null || target == null) {
            throw new ArgumentException("Origin tile: " + origin + " target: " + target);
        }

        // A*

        // setup first node
        origin.originDistance = 0;
        origin.targetDistance = (target.tile.transform.position - origin.tile.transform.position).magnitude;
        origin.cost = origin.originDistance + origin.targetDistance;

        toSearch = new List<Node>() { origin };
        processed = new List<Node>();

        while (toSearch.Any()) {
            // Find lowest cost
            Node current = toSearch[0];
            foreach (Node node in toSearch) {
                if (node.cost < current.cost || Math.Abs(node.cost - current.cost) < 0.1f && node.targetDistance < current.targetDistance) {
                    current = node;
                }
            }

            processed.Add(current);
            toSearch.Remove(current);

            if (current == target) {
                print("TARGET FOUND");
                List<Node> path = new List<Node>();
                Node currentPathTile = target;
                while (currentPathTile != origin) {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.connection;
                }

                path.Add(origin);
                return path;
            }


            foreach (Node neighbour in current.neighbours) {
                if (!processed.Contains(neighbour)) {
                    bool inSearch = toSearch.Contains(neighbour);

                    float costToNeighbour = current.originDistance + 1;
                    if (!inSearch || costToNeighbour < neighbour.originDistance) {
                        neighbour.originDistance = costToNeighbour;
                        neighbour.connection = current;

                        if (!inSearch) {
                            neighbour.targetDistance = (target.tile.transform.position - neighbour.tile.transform.position).magnitude;
                            neighbour.cost = neighbour.originDistance + neighbour.targetDistance;
                            toSearch.Add(neighbour);
                        }
                    }
                }
            }
        }

        return null;
    }

    Node[,] BuildNodeArray() {
        // build node array
        Node[,] nodes = new Node[grid.GetLength(0), grid.GetLength(1)];

        // create nodes
        for (int x = 0; x < grid.GetLength(0); x++) {
            for (int y = 0; y < grid.GetLength(1); y++) {
                if (grid[x, y] != null && grid[x, y].selectable) {
                    nodes[x, y] = new Node();
                }
            }
        }

        // assign neighbours
        for (int x = 0; x < nodes.GetLength(0); x++) {
            for (int y = 0; y < nodes.GetLength(1); y++) {
                if (nodes[x, y] == null) continue;

                List<Node> neighbours = new List<Node>();

                //up
                if (y + 1 < grid.GetLength(0)) {
                    if (nodes[x, y + 1] != null)
                        neighbours.Add(nodes[x, y + 1]);
                }

                //down
                if (y - 1 >= 0) {
                    if (nodes[x, y - 1] != null)
                        neighbours.Add(nodes[x, y - 1]);
                }

                //right
                if (x + 1 < grid.GetLength(0)) {
                    if (nodes[x + 1, y] != null)
                        neighbours.Add(nodes[x + 1, y]);
                }

                //left
                if (x - 1 >= 0) {
                    if (nodes[x - 1, y] != null)
                        neighbours.Add(nodes[x - 1, y]);
                }

                nodes[x, y].tile = grid[x, y];
                nodes[x, y].neighbours = neighbours;
            }
        }

        return nodes;
    }

    void GenerateGrid() {
        // Read map file
        int fileMapSize = mapFile.text.Split('\n').Length;
        string[,] mapStringArray = new string[fileMapSize, fileMapSize];


        int tx = 0;
        int ty = 0;
        foreach (string line in mapFile.text.Split('\n')) {
            foreach (string c in line.Split(' ')) {
                mapStringArray[tx, ty] = Utils.RemoveSpecialCharacters(c);
                ty++;
            }

            ty = 0;
            tx++;
        }

        mapStringArray = Utils.RotateMatrix90Degrees(mapStringArray);

        // print(Utils.MatrixToString(mapStringArray));

        // generate tile
        grid = new Tile[fileMapSize, fileMapSize];
        for (int x = 0; x < grid.GetLength(0); x++) {
            for (int y = 0; y < grid.GetLength(1); y++) {
                Transform selectedPrefab = null;
                bool selectable = false;
                switch (mapStringArray[x, y]) {
                    case "o":
                        selectedPrefab = tilePrefab;
                        selectable = true;
                        break;
                    case "m":
                        selectedPrefab = mountainTilePrefab;
                        break;
                    default:
                        break;
                }

                if (selectedPrefab != null) {
                    Transform tile = Instantiate(selectedPrefab, new Vector3(x, 0, y), Quaternion.identity);
                    tile.name = x + "," + y;
                    Transform display = Instantiate(coordTextPrefab, tile.position, coordTextPrefab.rotation);
                    display.Translate(Vector3.forward * -0.13f, Space.Self);
                    display.SetParent(GameObject.Find("CoordCanvas").transform);
                    display.GetComponent<TextMeshProUGUI>().text = x + "," + y;
                    tile.SetParent(transform);

                    Tile t = tile.GetComponent<Tile>();
                    t.x = x;
                    t.y = y;
                    t.selectable = selectable;
                    grid[x, y] = t;
                }
            }
        }

        // Add neighbours to tiles
        for (int x = 0; x < grid.GetLength(0); x++) {
            for (int y = 0; y < grid.GetLength(1); y++) {
                if (grid[x, y] == null || !grid[x, y].selectable) continue;

                List<Tile> neighbours = new List<Tile>();

                //up
                if (y + 1 < grid.GetLength(0)) {
                    if (grid[x, y + 1] != null && grid[x, y + 1].selectable)
                        neighbours.Add(grid[x, y + 1]);
                }

                //down
                if (y - 1 >= 0) {
                    if (grid[x, y - 1] != null && grid[x, y - 1].selectable)
                        neighbours.Add(grid[x, y - 1]);
                }

                //right
                if (x + 1 < grid.GetLength(0)) {
                    if (grid[x + 1, y] != null && grid[x + 1, y].selectable)
                        neighbours.Add(grid[x + 1, y]);
                }

                //left
                if (x - 1 >= 0) {
                    if (grid[x - 1, y] != null && grid[x - 1, y].selectable)
                        neighbours.Add(grid[x - 1, y]);
                }

                grid[x, y].neighbours = neighbours;
            }
        }
    }

    public class Node {
        public Node connection;
        public Tile tile;
        public float originDistance;
        public float targetDistance;
        public float cost;
        public List<Node> neighbours;

        public Node() {
            originDistance = float.MaxValue;
        }

        public override string ToString() {
            string n = "";
            foreach (Node neighbour in neighbours) {
                n += neighbour.tile.gameObject.name + " ";
            }

            return tile + " " + n;
        }
    }
}