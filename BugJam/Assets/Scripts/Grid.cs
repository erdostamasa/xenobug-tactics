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
        //
        // foreach (Tile tile in grid) {
        //     print(tile.gameObject.name);
        // }
    }


    public void ShowPath(Tile origin, Tile target) {
        Node[,] nodes = BuildNodeArray();

        print(Utils.MatrixToString(nodes));
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

    private class Node {
        public Tile tile;
        public float originDistance;
        public float targetDistance;
        public float cost;
        public List<Node> neighbours;

        public override string ToString() {
            string n = "";
            foreach (Node neighbour in neighbours) {
                n += neighbour.tile.gameObject.name + " ";
            }

            return tile + " " + n;
        }
    }
}