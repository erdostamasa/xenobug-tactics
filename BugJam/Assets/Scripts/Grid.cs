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
    [SerializeField] Transform backgroundTransform;
    [SerializeField] Transform coordTextPrefab;
    [SerializeField] TextAsset mapFile;


    [SerializeField] TextAsset defaultMap;

    public Tile[,] grid;
    Node[,] nodes;
    //Tile selectedTile;

    void Awake() {
        instance = this;
    }

    void Start() {
        if (LevelHolder.instance == null) {
            mapFile = defaultMap;
        }
        else {
            mapFile = LevelHolder.instance.GetLevelText();
        }
        //GenerateGrid();


        // Setup background cube
    }


    public void SpawnUnit(int x, int y, UnitDescriptor unit) {
        Unit spawned = Instantiate(unit.unitPrefab, grid[x, y].unitPosition.position, unit.unitPrefab.rotation).GetComponent<Unit>();
        spawned.transform.SetParent(transform);
        grid[x, y].unit = spawned;
        spawned.currentTile = grid[x, y];
        spawned.owner = unit.owner;
        spawned.SetAttackPattern(unit.attackPattern);
        if (unit.owner == Unit.Owner.ENEMY) {
            GameManager.instance.opponent.units.Add(spawned);
        }
        else if (unit.owner == Unit.Owner.PLAYER) {
            GameManager.instance.player.units.Add(spawned);
        }

        EventManager.instance.UnitCreated(spawned);
    }

    public void SpawnUnitWithTile(int x, int y, UnitDescriptor unit) {
        // spawn tile
        Transform tile = Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
        tile.name = x + "," + y;
        // Transform display = Instantiate(coordTextPrefab, tile.position, coordTextPrefab.rotation);
        // display.Translate(Vector3.forward * -0.13f, Space.Self);
        // display.SetParent(GameObject.Find("CoordCanvas").transform);
        // display.GetComponent<TextMeshProUGUI>().text = x + "," + y;
        tile.SetParent(transform);

        Tile t = tile.GetComponent<Tile>();
        t.x = x;
        t.y = y;
        t.selectable = true;
        t.walkable = true;
        grid[x, y] = t;

        //spawn unit
        Unit spawned = Instantiate(unit.unitPrefab, grid[x, y].unitPosition.position, unit.unitPrefab.rotation).GetComponent<Unit>();
        spawned.transform.SetParent(transform);
        grid[x, y].unit = spawned;
        spawned.currentTile = grid[x, y];
        spawned.owner = unit.owner;
        spawned.SetAttackPattern(unit.attackPattern);
        if (unit.owner == Unit.Owner.ENEMY) {
            GameManager.instance.opponent.units.Add(spawned);
        }
        else if (unit.owner == Unit.Owner.PLAYER) {
            GameManager.instance.player.units.Add(spawned);
        }

        EventManager.instance.UnitCreated(spawned);
    }

    // Return all tiles withing {range} steps
    public List<Tile> GetReachableInRange(Tile originTile, int distance) {
        // Find tiles withing range
        List<Node> result = GetNeighboursRecursive(nodes[originTile.x, originTile.y], distance);
        result = result.Distinct().ToList();

        // Remove origin tile
        if (result.Contains(nodes[originTile.x, originTile.y])) {
            result.Remove(nodes[originTile.x, originTile.y]);
        }

        // Remove tiles that cant be reached    
        List<Node> toRemove = new List<Node>();
        foreach (Node node in result) {
            if (GetPath(originTile, node.tile) == null) {
                toRemove.Add(node);
            }
        }

        result = result.Except(toRemove).ToList();


        List<Tile> tileResult = result.Select(n => n.tile).ToList();
        return tileResult;
    }

    List<Node> GetNeighboursRecursive(Node center, int depth) {
        if (depth < 0) {
            throw new ArgumentException("Depth must be non negative: " + depth);
        }

        if (depth == 0) {
            return new List<Node>();
        }

        List<Node> neighbours = new List<Node>();

        int x = center.tile.x;
        int y = center.tile.y;

        List<(int, int)> coords = new List<(int, int)>();


        //up
        if (y + 1 < grid.GetLength(0)) {
            coords.Add((x, y + 1));
        }

        //down
        if (y - 1 >= 0) {
            coords.Add((x, y - 1));
        }

        //right
        if (x + 1 < grid.GetLength(0)) {
            coords.Add((x + 1, y));
        }

        //left
        if (x - 1 >= 0) {
            coords.Add((x - 1, y));
        }

        foreach ((int, int) coord in coords) {
            if (nodes[coord.Item1, coord.Item2] == null) continue;
            Node c = nodes[coord.Item1, coord.Item2];
            if (!neighbours.Contains(c) && c.tile.walkable && c.tile.unit == null) {
                neighbours.Add(c);
            }
        }

        List<Node> newList = new List<Node>();
        foreach (Node neighbour in neighbours) {
            newList.AddRange(GetNeighboursRecursive(neighbour, depth - 1));
        }

        neighbours.AddRange(newList);


        return neighbours;
    }

    // Returns list of tiles on a path to target
    public List<Tile> GetPath(Tile originTile, Tile targetTile) {
        //find start and target
        Node origin = null;
        Node target = null;
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
            return new List<Tile>() { originTile };
            //throw new ArgumentException("Origin tile: " + origin + " target: " + target);
        }

        // A*

        // setup first node
        origin.originDistance = 0;
        origin.targetDistance = (target.tile.transform.position - origin.tile.transform.position).magnitude;
        origin.cost = origin.originDistance + origin.targetDistance;

        List<Node> toSearch = new List<Node>() { origin };
        List<Node> processed = new List<Node>();

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
                //print("TARGET FOUND");
                List<Tile> path = new List<Tile>();
                Node currentPathTile = target;
                while (currentPathTile != origin) {
                    path.Add(currentPathTile.tile);
                    currentPathTile = currentPathTile.connection;
                }

                return path;
            }


            foreach (Node neighbour in current.neighbours) {
                if (!processed.Contains(neighbour) && neighbour.tile.walkable && neighbour.tile.unit == null) {
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

    // Setup Node array for pathfinding
    void BuildNodeArray() {
        // build node array
        nodes = new Node[grid.GetLength(0), grid.GetLength(1)];

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
    }

    public void GenerateGrid() {
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
                bool walkable = false;
                bool unitSpawned = false;
                switch (mapStringArray[x, y]) {
                    case "o":
                        selectedPrefab = tilePrefab;
                        selectable = true;
                        walkable = true;
                        break;
                    case "m":
                        selectedPrefab = mountainTilePrefab;
                        break;
                    case "x":
                        break;
                    case "l":
                        unitSpawned = true;
                        SpawnUnitWithTile(x, y, GameManager.instance.playerLight);
                        break;
                    case "b":
                        unitSpawned = true;
                        SpawnUnitWithTile(x, y, GameManager.instance.enemy);
                        break;
                    default:
                        Debug.LogError("WRONG INPUT STRING");
                        break;
                }

                if (selectedPrefab != null && !unitSpawned) {
                    Transform tile = Instantiate(selectedPrefab, new Vector3(x, 0, y), Quaternion.identity);
                    tile.name = x + "," + y;
                    // Transform display = Instantiate(coordTextPrefab, tile.position, coordTextPrefab.rotation);
                    // display.Translate(Vector3.forward * -0.13f, Space.Self);
                    // display.SetParent(GameObject.Find("CoordCanvas").transform);
                    // display.GetComponent<TextMeshProUGUI>().text = x + "," + y;
                    tile.SetParent(transform);

                    Tile t = tile.GetComponent<Tile>();
                    t.x = x;
                    t.y = y;
                    t.selectable = selectable;
                    t.walkable = walkable;
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

        BuildNodeArray();
        float pos = (grid.GetLength(0) % 2 == 0) ? grid.GetLength(0) / 2f - 0.5f : grid.GetLength(0) / 2;
        backgroundTransform.position = new Vector3(pos, -0.41f, pos);
        backgroundTransform.localScale = new Vector3(grid.GetLength(0), 1, grid.GetLength(0));
    }

    // Node class used for pathfinding
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
            return tile.ToString();
        }
    }
}